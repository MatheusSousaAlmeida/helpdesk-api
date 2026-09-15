using HelpDesk.API.Domain.Entities;
namespace HelpDesk.API.Domain.Interfaces
{
 public interface IUsuarioRepository
 {
  Task<IEnumerable<Usuario>> ObterTodosAsync(int pageNumber,int pageSize); Task<Usuario?> ObterUmAsync(int id); Task<bool> ExisteEmailAsync(string email); Task<Usuario> AdicionarAsync(Usuario entity); Task<Usuario?> EditarAsync(int id,Usuario entity); Task<Usuario?> DeletarAsync(int id);
 }
}
