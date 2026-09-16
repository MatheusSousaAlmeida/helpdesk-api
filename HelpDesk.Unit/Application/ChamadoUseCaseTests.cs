using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.UseCases;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HelpDesk.Unit.Application;

public class ChamadoUseCaseTests
{
    private readonly Mock<IChamadoRepository> _chamadoRepository = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepository = new();
    private readonly Mock<ITecnicoRepository> _tecnicoRepository = new();

    private ChamadoUseCase CriarUseCase() => new(
        _chamadoRepository.Object,
        _usuarioRepository.Object,
        _tecnicoRepository.Object,
        NullLogger<ChamadoUseCase>.Instance);

    [Fact]
    public async Task AdicionarChamado_UsuarioAtivo_DeveIniciarComoAberto()
    {
        _usuarioRepository.Setup(x => x.ObterUmAsync(1))
            .ReturnsAsync(new Usuario {
                IdUsuario = 1, 
                Nome = "Usuario", 
                Email = "u@teste.com", 
                Departamento = "TI", 
                Ativo = true 
            });
        _chamadoRepository.Setup(x => x.AdicionarAsync(It.IsAny<Chamado>()))
            .ReturnsAsync((Chamado c) => c);

        var dto = CriarDto();
        var result = await CriarUseCase().AdicionarChamadoAsync(dto);

        Assert.Equal("Aberto", result.Status);
        Assert.NotEqual(default, result.DataAbertura);
        _chamadoRepository.Verify(x => x.AdicionarAsync(It.IsAny<Chamado>()), Times.Once);
    }

    [Fact]
    public async Task AdicionarChamado_UsuarioInativo_DeveFalhar()
    {
        _usuarioRepository.Setup(x => x.ObterUmAsync(1))
            .ReturnsAsync(new Usuario { 
                IdUsuario = 1, 
                Nome = "Usuario", 
                Email = "u@teste.com", 
                Departamento = "TI", 
                Ativo = false 
            });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => CriarUseCase().AdicionarChamadoAsync(CriarDto()));

        Assert.Contains("inativo", exception.Message.ToLowerInvariant());
    }

    [Fact]
    public async Task AdicionarChamado_PrioridadeInvalida_DeveFalhar()
    {
        _usuarioRepository.Setup(x => x.ObterUmAsync(1))
            .ReturnsAsync(new Usuario { 
                IdUsuario = 1, 
                Nome = "Usuario", 
                Email = "u@teste.com", 
                Departamento = "TI", 
                Ativo = true 
            });

        var dto = CriarDto();
        dto.Prioridade = "Urgentissima";

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CriarUseCase().AdicionarChamadoAsync(dto));
    }

    [Fact]
    public async Task EditarChamado_AbertoParaFechado_DeveFalhar()
    {
        _chamadoRepository.Setup(x => x.ObterUmAsync(10))
            .ReturnsAsync(new Chamado
            {
                IdChamado = 10,
                IdUsuario = 1,
                Titulo = "Teste",
                Descricao = "Descricao teste",
                Prioridade = "Alta",
                Status = "Aberto",
                DataAbertura = DateTime.UtcNow
            });
        _usuarioRepository.Setup(x => x.ObterUmAsync(1))
            .ReturnsAsync(new Usuario { 
                IdUsuario = 1, 
                Nome = "Usuario", 
                Email = "u@teste.com", 
                Departamento = "TI", 
                Ativo = true 
            });

        var dto = CriarDto();
        dto.IdChamado = 10;
        dto.Status = "Fechado";

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => CriarUseCase().EditarChamadoAsync(10, dto));
    }

    [Fact]
    public async Task EditarChamado_ResolvidoParaFechado_DevePreencherDataFechamento()
    {
        var chamado = new Chamado
        {
            IdChamado = 10,
            IdUsuario = 1,
            Titulo = "Teste",
            Descricao = "Descricao teste",
            Prioridade = "Alta",
            Status = "Resolvido",
            DataAbertura = DateTime.UtcNow
        };
        _chamadoRepository.Setup(x => x.ObterUmAsync(10)).ReturnsAsync(chamado);
        _usuarioRepository.Setup(x => x.ObterUmAsync(1))
            .ReturnsAsync(new Usuario { 
                IdUsuario = 1, 
                Nome = "Usuario", 
                Email = "u@teste.com", 
                Departamento = "TI", 
                Ativo = true 
            });
        _chamadoRepository.Setup(x => x.EditarAsync(10, It.IsAny<Chamado>()))
            .ReturnsAsync((int _, Chamado c) => c);

        var dto = CriarDto();
        dto.IdChamado = 10;
        dto.Status = "Fechado";

        var result = await CriarUseCase().EditarChamadoAsync(10, dto);

        Assert.NotNull(result);
        Assert.Equal("Fechado", result!.Status);
        Assert.NotNull(result.DataFechamento);
    }

    [Fact]
    public async Task Listagem_PageSizeMaiorQue100_DeveLimitarEm100()
    {
        _chamadoRepository.Setup(x => x.ObterTodosAsync(1, 100))
            .ReturnsAsync(Array.Empty<Chamado>());

        await CriarUseCase().ObterTodosChamadosAsync(1, 500);

        _chamadoRepository.Verify(x => x.ObterTodosAsync(1, 100), Times.Once);
    }

    private static ChamadoRequestDto CriarDto() => new()
    {
        IdChamado = 10,
        IdUsuario = 1,
        Titulo = "Falha no acesso",
        Descricao = "Usuario relata falha no acesso ao sistema.",
        Prioridade = "Alta"
    };
}
