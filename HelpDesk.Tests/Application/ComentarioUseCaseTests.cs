using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.UseCases;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace HelpDesk.Tests.Application;

public class ComentarioUseCaseTests
{
    [Fact]
    public async Task AdicionarComentario_ChamadoFechado_DeveFalhar()
    {
        var comentarios = new Mock<IComentarioRepository>();
        var chamados = new Mock<IChamadoRepository>();
        chamados.Setup(x => x.ObterUmAsync(1)).ReturnsAsync(new Chamado
        {
            IdChamado = 1,
            IdUsuario = 1,
            Titulo = "Chamado",
            Descricao = "Descricao",
            Prioridade = "Baixa",
            Status = "Fechado",
            DataAbertura = DateTime.UtcNow
        });

        var useCase = new ComentarioUseCase(comentarios.Object, chamados.Object, NullLogger<ComentarioUseCase>.Instance);
        var dto = new ComentarioRequestDto { IdComentario = 1, IdChamado = 1, Autor = "Tecnico", Texto = "Teste" };

        await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.AdicionarComentarioAsync(dto));
    }
}
