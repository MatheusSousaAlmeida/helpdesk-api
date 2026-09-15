using HelpDesk.API.Domain.Entities;
namespace HelpDesk.API.Domain.Interfaces
{
 public interface IComentarioRepository
 {
  Task<IEnumerable<Comentario>> ObterTodosAsync(int pageNumber,int pageSize); Task<Comentario?> ObterUmAsync(int id); Task<IEnumerable<Comentario>> ObterPorChamadoAsync(int idChamado,int pageNumber,int pageSize); Task<Comentario> AdicionarAsync(Comentario entity); Task<Comentario?> EditarAsync(int id,Comentario entity); Task<Comentario?> DeletarAsync(int id);
 }
}
