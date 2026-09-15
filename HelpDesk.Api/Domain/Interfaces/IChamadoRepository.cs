using HelpDesk.API.Domain.Entities;
namespace HelpDesk.API.Domain.Interfaces
{
 public interface IChamadoRepository
 {
  Task<IEnumerable<Chamado>> ObterTodosAsync(int pageNumber,int pageSize); Task<Chamado?> ObterUmAsync(int id); Task<IEnumerable<Chamado>> ObterPorUsuarioAsync(int idUsuario,int pageNumber,int pageSize); Task<IEnumerable<Chamado>> ObterPorTecnicoAsync(int idTecnico,int pageNumber,int pageSize); Task<Chamado> AdicionarAsync(Chamado entity); Task<Chamado?> EditarAsync(int id,Chamado entity); Task<Chamado?> DeletarAsync(int id);
 }
}
