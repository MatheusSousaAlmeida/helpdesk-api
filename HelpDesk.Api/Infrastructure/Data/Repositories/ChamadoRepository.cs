using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.API.Infrastructure.Data.Repositories
{
 public class ChamadoRepository:IChamadoRepository
 {
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Infrastructure");private readonly ApplicationContext _context;public ChamadoRepository(ApplicationContext context)=>_context=context;
  private IQueryable<Chamado> Query()=>_context.Chamados.AsNoTracking().Include(x=>x.Usuario).Include(x=>x.Tecnico);
  public async Task<IEnumerable<Chamado>> ObterTodosAsync(int p,int s){using var a=ActivitySource.StartActivity("ChamadoRepository.ObterTodosAsync");return await Query().OrderByDescending(x=>x.DataAbertura).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Chamado?> ObterUmAsync(int id){using var a=ActivitySource.StartActivity("ChamadoRepository.ObterUmAsync");return await Query().FirstOrDefaultAsync(x=>x.IdChamado==id);}
  public async Task<IEnumerable<Chamado>> ObterPorUsuarioAsync(int id,int p,int s){using var a=ActivitySource.StartActivity("ChamadoRepository.ObterPorUsuarioAsync");return await Query().Where(x=>x.IdUsuario==id).OrderByDescending(x=>x.DataAbertura).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<IEnumerable<Chamado>> ObterPorTecnicoAsync(int id,int p,int s){using var a=ActivitySource.StartActivity("ChamadoRepository.ObterPorTecnicoAsync");return await Query().Where(x=>x.IdTecnico==id).OrderByDescending(x=>x.DataAbertura).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Chamado> AdicionarAsync(Chamado e){using var a=ActivitySource.StartActivity("ChamadoRepository.AdicionarAsync");_context.Chamados.Add(e);await _context.SaveChangesAsync();return e;}
  public async Task<Chamado?> EditarAsync(int id,Chamado e){using var a=ActivitySource.StartActivity("ChamadoRepository.EditarAsync");if(id!=e.IdChamado)return null;_context.Chamados.Update(e);await _context.SaveChangesAsync();return e;}
  public async Task<Chamado?> DeletarAsync(int id){using var a=ActivitySource.StartActivity("ChamadoRepository.DeletarAsync");var e=await _context.Chamados.FirstOrDefaultAsync(x=>x.IdChamado==id);if(e is null)return null;_context.Chamados.Remove(e);await _context.SaveChangesAsync();return e;}
 }
}
