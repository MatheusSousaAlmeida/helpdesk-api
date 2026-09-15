using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.API.Infrastructure.Data.Repositories
{
 public class TecnicoRepository:ITecnicoRepository
 {
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Infrastructure");private readonly ApplicationContext _context;public TecnicoRepository(ApplicationContext context)=>_context=context;
  public async Task<IEnumerable<Tecnico>> ObterTodosAsync(int p,int s){using var a=ActivitySource.StartActivity("TecnicoRepository.ObterTodosAsync");return await _context.Tecnicos.AsNoTracking().OrderBy(x=>x.IdTecnico).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Tecnico?> ObterUmAsync(int id){using var a=ActivitySource.StartActivity("TecnicoRepository.ObterUmAsync");return await _context.Tecnicos.AsNoTracking().FirstOrDefaultAsync(x=>x.IdTecnico==id);}
  public async Task<bool> ExisteEmailAsync(string email){using var a=ActivitySource.StartActivity("TecnicoRepository.ExisteEmailAsync");return await _context.Tecnicos.AnyAsync(x=>x.Email==email);}
  public async Task<Tecnico> AdicionarAsync(Tecnico e){using var a=ActivitySource.StartActivity("TecnicoRepository.AdicionarAsync");_context.Tecnicos.Add(e);await _context.SaveChangesAsync();return e;}
  public async Task<Tecnico?> EditarAsync(int id,Tecnico e){using var a=ActivitySource.StartActivity("TecnicoRepository.EditarAsync");if(id!=e.IdTecnico)return null;_context.Tecnicos.Update(e);await _context.SaveChangesAsync();return e;}
  public async Task<Tecnico?> DeletarAsync(int id){using var a=ActivitySource.StartActivity("TecnicoRepository.DeletarAsync");var e=await _context.Tecnicos.FirstOrDefaultAsync(x=>x.IdTecnico==id);if(e is null)return null;_context.Tecnicos.Remove(e);await _context.SaveChangesAsync();return e;}
 }
}
