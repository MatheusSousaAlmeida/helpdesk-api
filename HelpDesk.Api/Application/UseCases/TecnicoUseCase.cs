using System.Diagnostics;
using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Application.Mappers;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
namespace HelpDesk.API.Application.UseCases
{
 public class TecnicoUseCase : ITecnicoUseCase
 {
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Application");
  private readonly ITecnicoRepository _tecnicoRepository; private readonly ILogger<TecnicoUseCase> _logger;
  public TecnicoUseCase(ITecnicoRepository tecnicoRepository,ILogger<TecnicoUseCase> logger){_tecnicoRepository=tecnicoRepository;_logger=logger;}
  public async Task<IEnumerable<Tecnico>> ObterTodosTecnicosAsync(int pageNumber,int pageSize){using var a=ActivitySource.StartActivity("TecnicoUseCase.ObterTodosTecnicosAsync");(pageNumber,pageSize)=Ajustar(pageNumber,pageSize);_logger.LogInformation("Obtendo tecnicos pagina {PageNumber} com tamanho {PageSize}",pageNumber,pageSize);return await _tecnicoRepository.ObterTodosAsync(pageNumber,pageSize);}
  public async Task<Tecnico?> ObterUmTecnicoAsync(int id){using var a=ActivitySource.StartActivity("TecnicoUseCase.ObterUmTecnicoAsync");var e=await _tecnicoRepository.ObterUmAsync(id);if(e is null)_logger.LogWarning("Tecnico {TecnicoId} nao encontrado",id);return e;}
  public async Task<Tecnico?> AdicionarTecnicoAsync(TecnicoRequestDto model){using var a=ActivitySource.StartActivity("TecnicoUseCase.AdicionarTecnicoAsync");if(await _tecnicoRepository.ExisteEmailAsync(model.Email)){_logger.LogWarning("E-mail {Email} ja cadastrado para tecnico",model.Email);return null;}return await _tecnicoRepository.AdicionarAsync(model.ToTecnicoEntity());}
  public async Task<Tecnico?> EditarTecnicoAsync(int id,TecnicoRequestDto model){using var a=ActivitySource.StartActivity("TecnicoUseCase.EditarTecnicoAsync");var e=await _tecnicoRepository.ObterUmAsync(id);if(e is null)return null;model.MapToExisting(e);return await _tecnicoRepository.EditarAsync(id,e);}
  public async Task<Tecnico?> DeletarTecnicoAsync(int id){using var a=ActivitySource.StartActivity("TecnicoUseCase.DeletarTecnicoAsync");return await _tecnicoRepository.DeletarAsync(id);}
  private static (int,int) Ajustar(int p,int s){if(p<=0)p=1;if(s<=0)s=10;if(s>100)s=100;return(p,s);}
 }
}
