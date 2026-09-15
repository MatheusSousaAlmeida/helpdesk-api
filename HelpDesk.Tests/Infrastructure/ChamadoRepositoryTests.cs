using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Infrastructure.Data;
using HelpDesk.API.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Tests.Infrastructure;

public class ChamadoRepositoryTests
{
    [Fact]
    public async Task ObterTodosAsync_DeveAplicarPaginacao()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase($"repo-{Guid.NewGuid()}")
            .Options;

        await using var context = new ApplicationContext(options);

        context.Usuarios.Add(new Usuario
        {
            IdUsuario = 1,
            Nome = "Usuario Teste",
            Email = "usuario.teste@empresa.com",
            Departamento = "TI",
            Ativo = true
        });

        context.Chamados.AddRange(Enumerable.Range(1, 15).Select(i => new Chamado
        {
            IdChamado = i,
            IdUsuario = 1,
            Titulo = $"Chamado {i}",
            Descricao = "Descricao",
            Prioridade = "Media",
            Status = "Aberto",
            DataAbertura = DateTime.UtcNow.AddMinutes(-i)
        }));
        await context.SaveChangesAsync();

        var repository = new ChamadoRepository(context);
        var result = (await repository.ObterTodosAsync(2, 5)).ToList();

        Assert.Equal(5, result.Count);
    }
}
