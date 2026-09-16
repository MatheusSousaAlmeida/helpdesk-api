using System.Diagnostics;
using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Application.Mappers;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;

namespace HelpDesk.API.Application.UseCases
{
    public class ComentarioUseCase : IComentarioUseCase
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Application");
        private readonly IComentarioRepository _comentarioRepository;
        private readonly IChamadoRepository _chamadoRepository;
        private readonly ILogger<ComentarioUseCase> _logger;

        public ComentarioUseCase(
            IComentarioRepository comentarioRepository,
            IChamadoRepository chamadoRepository,
            ILogger<ComentarioUseCase> logger)
        {
            _comentarioRepository = comentarioRepository;
            _chamadoRepository = chamadoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Comentario>> ObterTodosComentariosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.ObterTodosComentariosAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);
            return await _comentarioRepository.ObterTodosAsync(pageNumber, pageSize);
        }

        public async Task<Comentario?> ObterUmComentarioAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.ObterUmComentarioAsync");
            return await _comentarioRepository.ObterUmAsync(id);
        }

        public async Task<IEnumerable<Comentario>> ObterComentariosPorChamadoAsync(
            int idChamado,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.ObterComentariosPorChamadoAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);
            return await _comentarioRepository.ObterPorChamadoAsync(idChamado, pageNumber, pageSize);
        }

        public async Task<Comentario> AdicionarComentarioAsync(ComentarioRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.AdicionarComentarioAsync");
            var chamado = await _chamadoRepository.ObterUmAsync(model.IdChamado);

            if (chamado is null)
                throw new InvalidOperationException("Chamado nao encontrado.");

            if (string.Equals(chamado.Status, "Fechado", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chamado fechado nao pode receber comentarios.");

            var entity = model.ToComentarioEntity();
            entity.DataCriacao = DateTime.UtcNow;

            _logger.LogInformation(
                "Adicionando comentario {ComentarioId} ao chamado {ChamadoId}",
                model.IdComentario,
                model.IdChamado);

            return await _comentarioRepository.AdicionarAsync(entity);
        }

        public async Task<Comentario?> EditarComentarioAsync(int id, ComentarioRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.EditarComentarioAsync");
            var entity = await _comentarioRepository.ObterUmAsync(id);

            if (entity is null) return null;

            var chamado = await _chamadoRepository.ObterUmAsync(model.IdChamado);

            if (chamado is null)
                throw new InvalidOperationException("Chamado nao encontrado.");

            if (string.Equals(chamado.Status, "Fechado", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Comentario de chamado fechado nao pode ser alterado.");

            model.MapToExisting(entity);
            return await _comentarioRepository.EditarAsync(id, entity);
        }

        public async Task<Comentario?> DeletarComentarioAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ComentarioUseCase.DeletarComentarioAsync");
            return await _comentarioRepository.DeletarAsync(id);
        }

        private static (int, int) Ajustar(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            return (pageNumber, pageSize);
        }
    }
}
