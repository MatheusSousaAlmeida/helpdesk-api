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
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Application");
        private readonly ITecnicoRepository _tecnicoRepository;
        private readonly ILogger<TecnicoUseCase> _logger;

        public TecnicoUseCase(
            ITecnicoRepository tecnicoRepository,
            ILogger<TecnicoUseCase> logger)
        {
            _tecnicoRepository = tecnicoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Tecnico>> ObterTodosTecnicosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("TecnicoUseCase.ObterTodosTecnicosAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);

            _logger.LogInformation(
                "Obtendo tecnicos pagina {PageNumber} com tamanho {PageSize}",
                pageNumber,
                pageSize);

            return await _tecnicoRepository.ObterTodosAsync(pageNumber, pageSize);
        }

        public async Task<Tecnico?> ObterUmTecnicoAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("TecnicoUseCase.ObterUmTecnicoAsync");
            var entity = await _tecnicoRepository.ObterUmAsync(id);

            if (entity is null)
                _logger.LogWarning("Tecnico {TecnicoId} nao encontrado", id);

            return entity;
        }

        public async Task<Tecnico?> AdicionarTecnicoAsync(TecnicoRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("TecnicoUseCase.AdicionarTecnicoAsync");

            if (await _tecnicoRepository.ExisteEmailAsync(model.Email))
            {
                _logger.LogWarning("E-mail {Email} ja cadastrado para tecnico", model.Email);
                return null;
            }

            return await _tecnicoRepository.AdicionarAsync(model.ToTecnicoEntity());
        }

        public async Task<Tecnico?> EditarTecnicoAsync(int id, TecnicoRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("TecnicoUseCase.EditarTecnicoAsync");
            var entity = await _tecnicoRepository.ObterUmAsync(id);

            if (entity is null) return null;

            model.MapToExisting(entity);
            return await _tecnicoRepository.EditarAsync(id, entity);
        }

        public async Task<Tecnico?> DeletarTecnicoAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("TecnicoUseCase.DeletarTecnicoAsync");
            return await _tecnicoRepository.DeletarAsync(id);
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
