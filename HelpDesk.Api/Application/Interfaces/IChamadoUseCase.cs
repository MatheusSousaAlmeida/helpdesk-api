using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;
namespace HelpDesk.API.Application.Interfaces
{
 public interface IChamadoUseCase
 {
  Task<IEnumerable<Chamado>> ObterTodosChamadosAsync(int pageNumber,int pageSize); Task<Chamado?> ObterUmChamadoAsync(int id); Task<IEnumerable<Chamado>> ObterChamadosPorUsuarioAsync(int idUsuario,int pageNumber,int pageSize); Task<IEnumerable<Chamado>> ObterChamadosPorTecnicoAsync(int idTecnico,int pageNumber,int pageSize); Task<Chamado> AdicionarChamadoAsync(ChamadoRequestDto model); Task<Chamado?> EditarChamadoAsync(int id,ChamadoRequestDto model); Task<Chamado?> DeletarChamadoAsync(int id);
 }
}
