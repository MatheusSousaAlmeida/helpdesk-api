using HelpDesk.API.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ComentarioRequestSample : IExamplesProvider<ComentarioRequestDto>
    {
        public ComentarioRequestDto GetExamples() => new()
        {
            IdComentario = 1,
            IdChamado = 1,
            Autor = "Ana Ribeiro",
            Texto = "Identificado problema na configuracao de rede. Atendimento em andamento."
        };
    }
}
