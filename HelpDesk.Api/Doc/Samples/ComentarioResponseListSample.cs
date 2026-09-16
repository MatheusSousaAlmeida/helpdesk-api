using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ComentarioResponseListSample : IExamplesProvider<IEnumerable<Comentario>>
    {
        public IEnumerable<Comentario> GetExamples() => new List<Comentario>
        {
            new()
            {
                IdComentario = 1,
                IdChamado = 1,
                Autor = "Ana Ribeiro",
                Texto = "Atendimento iniciado.",
                DataCriacao = new DateTime(2026, 9, 15, 9, 10, 0)
            },
            new()
            {
                IdComentario = 2,
                IdChamado = 1,
                Autor = "Ana Ribeiro",
                Texto = "Configuracao de rede corrigida.",
                DataCriacao = new DateTime(2026, 9, 15, 9, 30, 0)
            }
        };
    }
}
