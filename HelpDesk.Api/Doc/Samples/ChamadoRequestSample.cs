using HelpDesk.API.Application.Dtos;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ChamadoRequestSample : IExamplesProvider<ChamadoRequestDto>
    {
        public ChamadoRequestDto GetExamples() => new()
        {
            IdChamado = 1,
            IdUsuario = 1,
            IdTecnico = 1,
            Titulo = "Computador sem acesso a internet",
            Descricao = "O usuario nao consegue acessar a rede corporativa.",
            Prioridade = "Alta",
            Status = "Aberto"
        };
    }
}
