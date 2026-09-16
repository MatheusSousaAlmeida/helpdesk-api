using HelpDesk.API.Domain.Entities;

namespace HelpDesk.Tests.Unit.Domain;

public class EntityTests
{
    [Fact]
    public void Usuario_DeveArmazenarDadosInformados()
    {
        var usuario = new Usuario
        {
            IdUsuario = 1,
            Nome = "Maria Silva",
            Email = "maria@empresa.com",
            Departamento = "Financeiro",
            Ativo = true
        };

        Assert.Equal(1, usuario.IdUsuario);
        Assert.Equal("Maria Silva", usuario.Nome);
        Assert.True(usuario.Ativo);
    }

    [Fact]
    public void Chamado_DevePermitirTecnicoNulo()
    {
        var chamado = new Chamado
        {
            IdChamado = 1,
            IdUsuario = 1,
            IdTecnico = null,
            Titulo = "Sem acesso ao sistema",
            Descricao = "Usuario nao consegue acessar o sistema.",
            Prioridade = "Alta",
            Status = "Aberto",
            DataAbertura = DateTime.UtcNow
        };

        Assert.Null(chamado.IdTecnico);
        Assert.Equal("Aberto", chamado.Status);
    }
}
