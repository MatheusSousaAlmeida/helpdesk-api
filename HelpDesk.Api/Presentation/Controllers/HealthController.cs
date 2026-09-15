using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;

namespace HelpDesk.API.Presentation.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthService;

        public HealthController(HealthCheckService healthService)
        {
            _healthService = healthService;
        }

        [HttpGet("live")]
        [SwaggerOperation(Summary = "Liveness da API")]
        public async Task<IActionResult> Live(CancellationToken ct)
        {
            var report = await _healthService.CheckHealthAsync(r => r.Tags.Contains("live"), ct);
            var result = BuildResult(report);
            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }

        [HttpGet("db")]
        [SwaggerOperation(Summary = "Readiness do banco Oracle")]
        public async Task<IActionResult> Database(CancellationToken ct)
        {
            var report = await _healthService.CheckHealthAsync(r => r.Tags.Contains("db"), ct);
            var result = BuildResult(report);
            return report.Status == HealthStatus.Healthy
                ? Ok(result)
                : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
        }

        private static object BuildResult(HealthReport report) => new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        };
    }
}
