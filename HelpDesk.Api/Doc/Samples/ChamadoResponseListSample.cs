using HelpDesk.API.Domain.Entities;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Doc.Samples
{
    public class ChamadoResponseListSample : IExamplesProvider<IEnumerable<Chamado>>
    {
        public IEnumerable<Chamado> GetExamples() => new List<Chamado>
        {
            new()
            {
                IdChamado = 1,
                IdUsuario = 1,
                IdTecnico = 1,
                Titulo = "Computador sem acesso a internet",
                Descricao = "O usuario nao consegue acessar a rede corporativa.",
                Prioridade = "Alta",
                Status = "Em Atendimento",
                DataAbertura = new DateTime(2026, 9, 15, 9, 0, 0)
            },
            new()
            {
                IdChamado = 2,
                IdUsuario = 2,
                Titulo = "Erro ao acessar sistema financeiro",
                Descricao = "O sistema apresenta erro ao autenticar o usuario.",
                Prioridade = "Critica",
                Status = "Aberto",
                DataAbertura = new DateTime(2026, 9, 15, 10, 0, 0)
            }
        };
    }
}
