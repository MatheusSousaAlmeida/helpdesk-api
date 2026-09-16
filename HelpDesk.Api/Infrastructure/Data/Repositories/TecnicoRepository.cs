using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Infrastructure.Data.Repositories
{
    public class TecnicoRepository : ITecnicoRepository
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Infrastructure");
        private readonly ApplicationContext _context;

        public TecnicoRepository(ApplicationContext context) => _context = context;

        public async Task<IEnumerable<Tecnico>> ObterTodosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.ObterTodosAsync");

            return await _context.Tecnicos
                .AsNoTracking()
                .OrderBy(x => x.IdTecnico)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Tecnico?> ObterUmAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.ObterUmAsync");

            return await _context.Tecnicos
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.IdTecnico == id);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.ExisteEmailAsync");
            return await _context.Tecnicos.AnyAsync(x => x.Email == email);
        }

        public async Task<Tecnico> AdicionarAsync(Tecnico entity)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.AdicionarAsync");
            _context.Tecnicos.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Tecnico?> EditarAsync(int id, Tecnico entity)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.EditarAsync");
            if (id != entity.IdTecnico) return null;

            _context.Tecnicos.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Tecnico?> DeletarAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("TecnicoRepository.DeletarAsync");
            var entity = await _context.Tecnicos.FirstOrDefaultAsync(x => x.IdTecnico == id);

            if (entity is null) return null;

            _context.Tecnicos.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
