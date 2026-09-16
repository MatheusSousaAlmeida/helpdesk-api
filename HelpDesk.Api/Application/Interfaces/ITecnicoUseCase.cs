using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Domain.Entities;

namespace HelpDesk.API.Application.Interfaces
{
    public interface ITecnicoUseCase
    {
        Task<IEnumerable<Tecnico>> ObterTodosTecnicosAsync(int pageNumber, int pageSize);
        Task<Tecnico?> ObterUmTecnicoAsync(int id);
        Task<Tecnico?> AdicionarTecnicoAsync(TecnicoRequestDto model);
        Task<Tecnico?> EditarTecnicoAsync(int id, TecnicoRequestDto model);
        Task<Tecnico?> DeletarTecnicoAsync(int id);
    }
}
