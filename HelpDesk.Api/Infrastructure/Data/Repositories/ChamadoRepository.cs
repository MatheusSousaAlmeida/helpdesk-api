using System.Diagnostics;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Infrastructure.Data.Repositories
{
    public class ChamadoRepository : IChamadoRepository
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Infrastructure");
        private readonly ApplicationContext _context;

        public ChamadoRepository(ApplicationContext context) => _context = context;

        private IQueryable<Chamado> Query()
        {
            return _context.Chamados
                .AsNoTracking()
                .Include(x => x.Usuario)
                .Include(x => x.Tecnico);
        }

        public async Task<IEnumerable<Chamado>> ObterTodosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.ObterTodosAsync");

            return await Query()
                .OrderByDescending(x => x.DataAbertura)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Chamado?> ObterUmAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.ObterUmAsync");
            return await Query().FirstOrDefaultAsync(x => x.IdChamado == id);
        }

        public async Task<IEnumerable<Chamado>> ObterPorUsuarioAsync(
            int idUsuario,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.ObterPorUsuarioAsync");

            return await Query()
                .Where(x => x.IdUsuario == idUsuario)
                .OrderByDescending(x => x.DataAbertura)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chamado>> ObterPorTecnicoAsync(
            int idTecnico,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.ObterPorTecnicoAsync");

            return await Query()
                .Where(x => x.IdTecnico == idTecnico)
                .OrderByDescending(x => x.DataAbertura)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Chamado> AdicionarAsync(Chamado entity)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.AdicionarAsync");
            _context.Chamados.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Chamado?> EditarAsync(int id, Chamado entity)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.EditarAsync");
            if (id != entity.IdChamado) return null;

            _context.Chamados.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Chamado?> DeletarAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ChamadoRepository.DeletarAsync");
            var entity = await _context.Chamados.FirstOrDefaultAsync(x => x.IdChamado == id);

            if (entity is null) return null;

            _context.Chamados.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
