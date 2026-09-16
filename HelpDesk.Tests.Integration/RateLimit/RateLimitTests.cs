using System.Net;
using HelpDesk.Tests.Integration.Fixtures;

namespace HelpDesk.Tests.Integration.RateLimit;

public class RateLimitTests : IClassFixture<RateLimitWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitTests(RateLimitWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SextaRequisicao_DeveRetornar429()
    {
        for (var i = 0; i < 5; i++)
        {
            var response = await _client.GetAsync("/api/usuarios?pageNumber=1&pageSize=10");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        var responseLimitada = await _client.GetAsync("/api/usuarios?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.TooManyRequests, responseLimitada.StatusCode);
    }
}
