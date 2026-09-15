using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class UsuarioResponseSample : IExamplesProvider<Usuario>
    {
        public Usuario GetExamples() => new()
        {
            IdUsuario = 1,
            Nome = "Maria Souza",
            Email = "maria.souza@empresa.com",
            Departamento = "Financeiro",
            Ativo = true
        };
    }
}
