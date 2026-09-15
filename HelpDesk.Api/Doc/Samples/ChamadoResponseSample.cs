using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ChamadoResponseSample : IExamplesProvider<Chamado>
    {
        public Chamado GetExamples() => new()
        {
            IdChamado = 1,
            IdUsuario = 1,
            IdTecnico = 1,
            Titulo = "Computador sem acesso a internet",
            Descricao = "O usuario nao consegue acessar a rede corporativa.",
            Prioridade = "Alta",
            Status = "Aberto",
            DataAbertura = new DateTime(2026, 9, 15, 9, 0, 0),
            DataAtualizacao = new DateTime(2026, 9, 15, 9, 15, 0)
        };
    }
}
