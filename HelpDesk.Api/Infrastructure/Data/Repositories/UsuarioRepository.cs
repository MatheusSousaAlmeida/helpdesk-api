using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace HelpDesk.API.Infrastructure.Data.Repositories
{
 public class UsuarioRepository:IUsuarioRepository
 {
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Infrastructure");private readonly ApplicationContext _context;public UsuarioRepository(ApplicationContext context)=>_context=context;
  public async Task<IEnumerable<Usuario>> ObterTodosAsync(int p,int s){using var a=ActivitySource.StartActivity("UsuarioRepository.ObterTodosAsync");return await _context.Usuarios.AsNoTracking().OrderBy(x=>x.IdUsuario).Skip((p-1)*s).Take(s).ToListAsync();}
  public async Task<Usuario?> ObterUmAsync(int id){using var a=ActivitySource.StartActivity("UsuarioRepository.ObterUmAsync");return await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(x=>x.IdUsuario==id);}
  public async Task<bool> ExisteEmailAsync(string email){using var a=ActivitySource.StartActivity("UsuarioRepository.ExisteEmailAsync");return await _context.Usuarios.AnyAsync(x=>x.Email==email);}
  public async Task<Usuario> AdicionarAsync(Usuario e){using var a=ActivitySource.StartActivity("UsuarioRepository.AdicionarAsync");_context.Usuarios.Add(e);await _context.SaveChangesAsync();return e;}
  public async Task<Usuario?> EditarAsync(int id,Usuario e){using var a=ActivitySource.StartActivity("UsuarioRepository.EditarAsync");if(id!=e.IdUsuario)return null;_context.Usuarios.Update(e);await _context.SaveChangesAsync();return e;}
  public async Task<Usuario?> DeletarAsync(int id){using var a=ActivitySource.StartActivity("UsuarioRepository.DeletarAsync");var e=await _context.Usuarios.FirstOrDefaultAsync(x=>x.IdUsuario==id);if(e is null)return null;_context.Usuarios.Remove(e);await _context.SaveChangesAsync();return e;}
 }
}
