using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Domain.Interfaces
{
    public interface ITecnicoRepository
    {
        Task<IEnumerable<Tecnico>> ObterTodosAsync(int pageNumber, int pageSize);
        Task<Tecnico?> ObterUmAsync(int id);
        Task<bool> ExisteEmailAsync(string email);
        Task<Tecnico> AdicionarAsync(Tecnico entity);
        Task<Tecnico?> EditarAsync(int id, Tecnico entity);
        Task<Tecnico?> DeletarAsync(int id);
    }
}
