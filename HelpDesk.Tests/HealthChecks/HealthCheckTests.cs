using System.Net;
using HelpDesk.Tests.Fixtures;

namespace HelpDesk.Tests.HealthChecks;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthControllerLive_DeveRetornar200()
    {
        var response = await _client.GetAsync("/api/health/live");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
