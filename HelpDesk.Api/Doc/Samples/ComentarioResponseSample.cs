using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ComentarioResponseSample : IExamplesProvider<Comentario>
    {
        public Comentario GetExamples() => new()
        {
            IdComentario = 1,
            IdChamado = 1,
            Autor = "Ana Ribeiro",
            Texto = "Identificado problema na configuracao de rede. Atendimento em andamento.",
            DataCriacao = new DateTime(2026, 9, 15, 9, 20, 0)
        };
    }
}
