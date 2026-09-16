using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Interfaces
{
    public interface IUsuarioUseCase
    {
        Task<IEnumerable<Usuario>> ObterTodosUsuariosAsync(int pageNumber, int pageSize);
        Task<Usuario?> ObterUmUsuarioAsync(int id);
        Task<Usuario?> AdicionarUsuarioAsync(UsuarioRequestDto model);
        Task<Usuario?> EditarUsuarioAsync(int id, UsuarioRequestDto model);
        Task<Usuario?> DeletarUsuarioAsync(int id);
    }
}
