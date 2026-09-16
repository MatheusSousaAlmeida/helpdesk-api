using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace HelpDesk.Integration.Fixtures;

public class RateLimitWebApplicationFactory : WebApplicationFactory<Program>
{
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

            var usuarioUseCase = new Mock<IUsuarioUseCase>();
            usuarioUseCase
                .Setup(x => x.ObterTodosUsuariosAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new[]
                {
                    new Usuario
                    {
                        IdUsuario = 1,
                        Nome = "Usuario Teste",
                        Email = "usuario.teste@empresa.com",
                        Departamento = "TI",
                        Ativo = true
                    }
                });

            services.AddSingleton(usuarioUseCase.Object);
        });
    }
}
