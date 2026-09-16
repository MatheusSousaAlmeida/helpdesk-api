using System.Diagnostics;
using System.IO.Compression;
using System.Threading.RateLimiting;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using HelpDesk.API.Infrastructure.Data;
using HelpDesk.API.Infrastructure.IoC;
using HelpDesk.API.Infrastructure.Observability;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

var logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
Directory.CreateDirectory(logDirectory);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "HelpDesk.API")
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine(logDirectory, "api-.log"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [CorrelationId:{CorrelationId}] [TraceId:{TraceId}] {SourceContext}{NewLine}    {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Services.AddSerilog();
Log.Information("Logging estruturado inicializado. Arquivos de log em {LogDirectory}", logDirectory);

builder.Services.AddHelpDeskInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.AddHealthChecks()
    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy("API em execucao."),
        tags: ["live"])
    .AddOracle(
        connectionString: builder.Configuration.GetConnectionString("OracleDbConnection") ?? string.Empty,
        name: "oracle",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["db"]);

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("politica_5_tentativas", context =>
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIp,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromSeconds(20),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var enableConsoleExporter = !bool.TryParse(
    builder.Configuration["Observability:EnableConsoleExporter"],
    out var consoleExporterEnabled) || consoleExporterEnabled;

var openTelemetry = builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("HelpDesk.API"));

openTelemetry.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource("HelpDesk.Application")
        .AddSource("HelpDesk.Infrastructure");

    if (enableConsoleExporter)
        tracing.AddConsoleExporter();
});

openTelemetry.WithMetrics(metrics =>
{
    metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddMeter(ApiMetrics.MeterName);

    if (enableConsoleExporter)
        metrics.AddConsoleExporter();
});

var applicationInsightsConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(applicationInsightsConnectionString))
{
    openTelemetry.UseAzureMonitor(options =>
    {
        options.ConnectionString = applicationInsightsConnectionString;
    });
}

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    const string correlationHeader = "X-Correlation-ID";
    var correlationId = context.Request.Headers[correlationHeader].FirstOrDefault();

    if (string.IsNullOrWhiteSpace(correlationId))
        correlationId = Guid.NewGuid().ToString("N");

    context.TraceIdentifier = correlationId;
    context.Response.Headers[correlationHeader] = correlationId;

    var traceId = Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    using (LogContext.PushProperty("CorrelationId", correlationId))
    using (LogContext.PushProperty("TraceId", traceId))
    {
        await next();
    }
});

app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (httpContext, _, exception) =>
    {
        if (exception is not null || httpContext.Response.StatusCode >= 500)
            return LogEventLevel.Error;

        return httpContext.Response.StatusCode >= 400
            ? LogEventLevel.Warning
            : LogEventLevel.Information;
    };
});

app.Use(async (context, next) =>
{
    var stopwatch = Stopwatch.StartNew();
    try
    {
        await next();
    }
    finally
    {
        stopwatch.Stop();
        var metrics = context.RequestServices.GetRequiredService<ApiMetrics>();
        metrics.Record(
            context.Request.Method,
            context.Request.Path.Value ?? "/",
            context.Response.StatusCode,
            stopwatch.Elapsed.TotalMilliseconds);
    }
});

app.UseResponseCompression();
app.UseRateLimiter();
app.UseAuthorization();

app.MapGet("/metrics", (ApiMetrics metrics) => Results.Ok(metrics.GetSnapshot()));
app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("OracleDbConnection");
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        try
        {
            dbContext.Database.Migrate();
            Log.Information("Migrations do banco de dados aplicadas/verificadas com sucesso");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Falha ao aplicar migrations. A API continuara em execucao para permitir diagnostico.");
        }
    }
}

app.Run();

public partial class Program { }
