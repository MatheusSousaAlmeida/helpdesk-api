using HelpDesk.API.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class UsuarioRequestSample : IExamplesProvider<UsuarioRequestDto>
    {
        public UsuarioRequestDto GetExamples() => new()
        {
            IdUsuario = 1,
            Nome = "Maria Souza",
            Email = "maria.souza@empresa.com",
            Departamento = "Financeiro",
            Ativo = true
        };
    }
}
