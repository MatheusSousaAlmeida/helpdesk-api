using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Infrastructure.Data.Repositories
{
    public class ComentarioRepository : IComentarioRepository
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Infrastructure");
        private readonly ApplicationContext _context;

        public ComentarioRepository(ApplicationContext context) => _context = context;

        public async Task<IEnumerable<Comentario>> ObterTodosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.ObterTodosAsync");

            return await _context.Comentarios
                .AsNoTracking()
                .Include(x => x.Chamado)
                .OrderByDescending(x => x.DataCriacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Comentario?> ObterUmAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.ObterUmAsync");

            return await _context.Comentarios
                .AsNoTracking()
                .Include(x => x.Chamado)
                .FirstOrDefaultAsync(x => x.IdComentario == id);
        }

        public async Task<IEnumerable<Comentario>> ObterPorChamadoAsync(
            int idChamado,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.ObterPorChamadoAsync");

            return await _context.Comentarios
                .AsNoTracking()
                .Where(x => x.IdChamado == idChamado)
                .OrderBy(x => x.DataCriacao)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Comentario> AdicionarAsync(Comentario entity)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.AdicionarAsync");
            _context.Comentarios.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Comentario?> EditarAsync(int id, Comentario entity)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.EditarAsync");
            if (id != entity.IdComentario) return null;

            _context.Comentarios.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Comentario?> DeletarAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ComentarioRepository.DeletarAsync");
            var entity = await _context.Comentarios.FirstOrDefaultAsync(x => x.IdComentario == id);

            if (entity is null) return null;

            _context.Comentarios.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
