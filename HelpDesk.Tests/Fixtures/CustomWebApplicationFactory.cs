using HelpDesk.API.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace HelpDesk.Tests.Fixtures;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUsuarioUseCase> UsuarioUseCaseMock { get; } = new();
    public Mock<ITecnicoUseCase> TecnicoUseCaseMock { get; } = new();
    public Mock<IChamadoUseCase> ChamadoUseCaseMock { get; } = new();
    public Mock<IComentarioUseCase> ComentarioUseCaseMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:OracleDbConnection"] = "User Id=test;Password=test;Data Source=localhost:1521/XEPDB1",
                ["ApplicationInsights:ConnectionString"] = string.Empty,
                ["Observability:EnableConsoleExporter"] = "false"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(IUsuarioUseCase));
            services.RemoveAll(typeof(ITecnicoUseCase));
            services.RemoveAll(typeof(IChamadoUseCase));
            services.RemoveAll(typeof(IComentarioUseCase));

            services.AddSingleton(UsuarioUseCaseMock.Object);
            services.AddSingleton(TecnicoUseCaseMock.Object);
            services.AddSingleton(ChamadoUseCaseMock.Object);
            services.AddSingleton(ComentarioUseCaseMock.Object);
        });
    }
}
