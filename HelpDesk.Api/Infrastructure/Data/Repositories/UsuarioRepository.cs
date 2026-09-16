using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Infrastructure");
        private readonly ApplicationContext _context;

        public UsuarioRepository(ApplicationContext context) => _context = context;

        public async Task<IEnumerable<Usuario>> ObterTodosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.ObterTodosAsync");

            return await _context.Usuarios
                .AsNoTracking()
                .OrderBy(x => x.IdUsuario)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Usuario?> ObterUmAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.ObterUmAsync");

            return await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdUsuario == id);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.ExisteEmailAsync");
            return await _context.Usuarios.AnyAsync(x => x.Email == email);
        }

        public async Task<Usuario> AdicionarAsync(Usuario entity)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.AdicionarAsync");
            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Usuario?> EditarAsync(int id, Usuario entity)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.EditarAsync");
            if (id != entity.IdUsuario) return null;

            _context.Usuarios.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Usuario?> DeletarAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("UsuarioRepository.DeletarAsync");
            var entity = await _context.Usuarios.FirstOrDefaultAsync(x => x.IdUsuario == id);

            if (entity is null) return null;

            _context.Usuarios.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
