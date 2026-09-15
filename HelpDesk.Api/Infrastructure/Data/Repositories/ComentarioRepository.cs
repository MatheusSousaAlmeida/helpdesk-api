using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.API.Infrastructure.Data.Repositories
{
 public class ComentarioRepository:IComentarioRepository
 {
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Infrastructure");private readonly ApplicationContext _context;public ComentarioRepository(ApplicationContext context)=>_context=context;
  public async Task<IEnumerable<Comentario>> ObterTodosAsync(int p,int s){using var a=ActivitySource.StartActivity("ComentarioRepository.ObterTodosAsync");return await _context.Comentarios.AsNoTracking().Include(x=>x.Chamado).OrderByDescending(x=>x.DataCriacao).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Comentario?> ObterUmAsync(int id){using var a=ActivitySource.StartActivity("ComentarioRepository.ObterUmAsync");return await _context.Comentarios.AsNoTracking().Include(x=>x.Chamado).FirstOrDefaultAsync(x=>x.IdComentario==id);}
  public async Task<IEnumerable<Comentario>> ObterPorChamadoAsync(int id,int p,int s){using var a=ActivitySource.StartActivity("ComentarioRepository.ObterPorChamadoAsync");return await _context.Comentarios.AsNoTracking().Where(x=>x.IdChamado==id).OrderBy(x=>x.DataCriacao).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Comentario> AdicionarAsync(Comentario e){using var a=ActivitySource.StartActivity("ComentarioRepository.AdicionarAsync");_context.Comentarios.Add(e);await _context.SaveChangesAsync();return e;}
  public async Task<Comentario?> EditarAsync(int id,Comentario e){using var a=ActivitySource.StartActivity("ComentarioRepository.EditarAsync");if(id!=e.IdComentario)return null;_context.Comentarios.Update(e);await _context.SaveChangesAsync();return e;}
  public async Task<Comentario?> DeletarAsync(int id){using var a=ActivitySource.StartActivity("ComentarioRepository.DeletarAsync");var e=await _context.Comentarios.FirstOrDefaultAsync(x=>x.IdComentario==id);if(e is null)return null;_context.Comentarios.Remove(e);await _context.SaveChangesAsync();return e;}
 }
}
