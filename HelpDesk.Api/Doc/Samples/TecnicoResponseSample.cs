using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class TecnicoResponseSample : IExamplesProvider<Tecnico>
    {
        public Tecnico GetExamples() => new()
        {
            IdTecnico = 1,
            Nome = "Ana Ribeiro",
            Email = "ana.ribeiro@empresa.com",
            Especialidade = "Infraestrutura",
            Ativo = true
        };
    }
}
