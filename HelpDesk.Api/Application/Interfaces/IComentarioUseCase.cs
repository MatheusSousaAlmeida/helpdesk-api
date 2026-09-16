using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Interfaces
{
    public interface IComentarioUseCase
    {
        Task<IEnumerable<Comentario>> ObterTodosComentariosAsync(int pageNumber, int pageSize);
        Task<Comentario?> ObterUmComentarioAsync(int id);
        Task<IEnumerable<Comentario>> ObterComentariosPorChamadoAsync(int idChamado, int pageNumber, int pageSize);
        Task<Comentario> AdicionarComentarioAsync(ComentarioRequestDto model);
        Task<Comentario?> EditarComentarioAsync(int id, ComentarioRequestDto model);
        Task<Comentario?> DeletarComentarioAsync(int id);
    }
}
