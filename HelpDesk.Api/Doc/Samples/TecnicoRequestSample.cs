using HelpDesk.API.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class TecnicoRequestSample : IExamplesProvider<TecnicoRequestDto>
    {
        public TecnicoRequestDto GetExamples() => new()
        {
            IdTecnico = 1,
            Nome = "Ana Ribeiro",
            Email = "ana.ribeiro@empresa.com",
            Especialidade = "Infraestrutura",
            Ativo = true
        };
    }
}
