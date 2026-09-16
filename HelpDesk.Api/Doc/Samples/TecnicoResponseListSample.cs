using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class TecnicoResponseListSample : IExamplesProvider<IEnumerable<Tecnico>>
    {
        public IEnumerable<Tecnico> GetExamples() => new List<Tecnico>
        {
            new()
            {
                IdTecnico = 1,
                Nome = "Ana Ribeiro",
                Email = "ana.ribeiro@empresa.com",
                Especialidade = "Infraestrutura",
                Ativo = true
            },
            new()
            {
                IdTecnico = 2,
                Nome = "Pedro Martins",
                Email = "pedro.martins@empresa.com",
                Especialidade = "Sistemas",
                Ativo = true
            }
        };
    }
}
