using System.Net;
using System.Net.Http.Json;
using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;
using HelpDesk.Tests.Fixtures;
using Moq;

namespace HelpDesk.Tests.Controllers;

public class HelpDeskApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HelpDeskApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostUsuario_Valido_DeveRetornarCreated()
    {
        var request = new UsuarioRequestDto
        {
            IdUsuario = 1,
            Nome = "Joao Silva",
            Email = "joao@empresa.com",
            Departamento = "Financeiro",
            Ativo = true
        };
        _factory.UsuarioUseCaseMock.Reset();
        _factory.UsuarioUseCaseMock
            .Setup(x => x.AdicionarUsuarioAsync(It.IsAny<UsuarioRequestDto>()))
            .ReturnsAsync(new Usuario
            {
                IdUsuario = request.IdUsuario,
                Nome = request.Nome,
                Email = request.Email,
                Departamento = request.Departamento,
                Ativo = true
            });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/usuarios", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostChamado_Valido_DeveRetornarCreated()
    {
        var request = new ChamadoRequestDto
        {
            IdChamado = 1,
            IdUsuario = 1,
            IdTecnico = 1,
            Titulo = "Erro no ERP",
            Descricao = "Usuario nao consegue acessar o ERP.",
            Prioridade = "Alta"
        };
        _factory.ChamadoUseCaseMock.Reset();
        _factory.ChamadoUseCaseMock
            .Setup(x => x.AdicionarChamadoAsync(It.IsAny<ChamadoRequestDto>()))
            .ReturnsAsync(new Chamado
            {
                IdChamado = 1,
                IdUsuario = 1,
                IdTecnico = 1,
                Titulo = request.Titulo,
                Descricao = request.Descricao,
                Prioridade = request.Prioridade,
                Status = "Aberto",
                DataAbertura = DateTime.UtcNow
            });

        using var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/chamados", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetChamados_SemRegistros_DeveRepassarPaginacaoERetornar204()
    {
        _factory.ChamadoUseCaseMock.Reset();
        _factory.ChamadoUseCaseMock
            .Setup(x => x.ObterTodosChamadosAsync(2, 5))
            .ReturnsAsync(Array.Empty<Chamado>());

        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/chamados?pageNumber=2&pageSize=5");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        _factory.ChamadoUseCaseMock.Verify(x => x.ObterTodosChamadosAsync(2, 5), Times.Once);
    }

    [Fact]
    public async Task GetChamadoInexistente_DeveRetornar404()
    {
        _factory.ChamadoUseCaseMock.Reset();
        _factory.ChamadoUseCaseMock
            .Setup(x => x.ObterUmChamadoAsync(999999))
            .ReturnsAsync((Chamado?)null);

        using var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/chamados/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
