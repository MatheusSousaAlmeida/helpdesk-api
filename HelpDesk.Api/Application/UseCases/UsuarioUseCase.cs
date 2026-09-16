using System.Diagnostics;
using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Application.Mappers;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;

namespace HelpDesk.API.Application.UseCases
{
    public class UsuarioUseCase : IUsuarioUseCase
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Application");
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioUseCase> _logger;

        public UsuarioUseCase(
            IUsuarioRepository usuarioRepository,
            ILogger<UsuarioUseCase> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> ObterTodosUsuariosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("UsuarioUseCase.ObterTodosUsuariosAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);

            _logger.LogInformation(
                "Obtendo usuarios pagina {PageNumber} com tamanho {PageSize}",
                pageNumber,
                pageSize);

            return await _usuarioRepository.ObterTodosAsync(pageNumber, pageSize);
        }

        public async Task<Usuario?> ObterUmUsuarioAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("UsuarioUseCase.ObterUmUsuarioAsync");
            activity?.SetTag("usuario.id", id);

            _logger.LogInformation("Obtendo usuario {UsuarioId} do repository", id);

            var entity = await _usuarioRepository.ObterUmAsync(id);

            if (entity is null)
                _logger.LogWarning("Usuario {UsuarioId} nao encontrado", id);

            return entity;
        }

        public async Task<Usuario?> AdicionarUsuarioAsync(UsuarioRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("UsuarioUseCase.AdicionarUsuarioAsync");
            _logger.LogInformation("Cadastrando usuario {UsuarioId}", model.IdUsuario);

            if (await _usuarioRepository.ExisteEmailAsync(model.Email))
            {
                _logger.LogWarning("E-mail {Email} ja cadastrado para usuario", model.Email);
                return null;
            }

            return await _usuarioRepository.AdicionarAsync(model.ToUsuarioEntity());
        }

        public async Task<Usuario?> EditarUsuarioAsync(int id, UsuarioRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("UsuarioUseCase.EditarUsuarioAsync");
            var entity = await _usuarioRepository.ObterUmAsync(id);

            if (entity is null)
            {
                _logger.LogWarning("Usuario {UsuarioId} nao encontrado para edicao", id);
                return null;
            }

            model.MapToExisting(entity);
            _logger.LogInformation("Atualizando usuario {UsuarioId}", id);

            return await _usuarioRepository.EditarAsync(id, entity);
        }

        public async Task<Usuario?> DeletarUsuarioAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("UsuarioUseCase.DeletarUsuarioAsync");
            _logger.LogInformation("Excluindo usuario {UsuarioId}", id);
            return await _usuarioRepository.DeletarAsync(id);
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
