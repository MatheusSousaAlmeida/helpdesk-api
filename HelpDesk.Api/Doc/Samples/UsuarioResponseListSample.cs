using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class UsuarioResponseListSample : IExamplesProvider<IEnumerable<Usuario>>
    {
        public IEnumerable<Usuario> GetExamples() => new List<Usuario>
        {
            new() { IdUsuario = 1, Nome = "Maria Souza", Email = "maria.souza@empresa.com", Departamento = "Financeiro", Ativo = true },
            new() { IdUsuario = 2, Nome = "Carlos Lima", Email = "carlos.lima@empresa.com", Departamento = "Operacoes", Ativo = true }
        };
    }
}
