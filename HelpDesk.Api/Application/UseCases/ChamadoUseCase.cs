using System.Diagnostics;
using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Application.Mappers;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Domain.Interfaces;

namespace HelpDesk.API.Application.UseCases
{
    public class ChamadoUseCase : IChamadoUseCase
    {
        private static readonly ActivitySource ActivitySource = new("HelpDesk.Application");
        private static readonly string[] PrioridadesValidas = ["Baixa", "Media", "Alta", "Critica"];
        private static readonly string[] StatusValidos = ["Aberto", "Em Atendimento", "Resolvido", "Fechado"];

        private readonly IChamadoRepository _chamadoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITecnicoRepository _tecnicoRepository;
        private readonly ILogger<ChamadoUseCase> _logger;

        public ChamadoUseCase(
            IChamadoRepository chamadoRepository,
            IUsuarioRepository usuarioRepository,
            ITecnicoRepository tecnicoRepository,
            ILogger<ChamadoUseCase> logger)
        {
            _chamadoRepository = chamadoRepository;
            _usuarioRepository = usuarioRepository;
            _tecnicoRepository = tecnicoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Chamado>> ObterTodosChamadosAsync(int pageNumber, int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.ObterTodosChamadosAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);

            _logger.LogInformation(
                "Obtendo chamados pagina {PageNumber} com tamanho {PageSize}",
                pageNumber,
                pageSize);

            return await _chamadoRepository.ObterTodosAsync(pageNumber, pageSize);
        }

        public async Task<Chamado?> ObterUmChamadoAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.ObterUmChamadoAsync");
            activity?.SetTag("chamado.id", id);

            _logger.LogInformation("Obtendo chamado {ChamadoId} do repository", id);

            var entity = await _chamadoRepository.ObterUmAsync(id);

            if (entity is null)
                _logger.LogWarning("Chamado {ChamadoId} nao encontrado", id);

            return entity;
        }

        public async Task<IEnumerable<Chamado>> ObterChamadosPorUsuarioAsync(
            int idUsuario,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.ObterChamadosPorUsuarioAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);
            return await _chamadoRepository.ObterPorUsuarioAsync(idUsuario, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Chamado>> ObterChamadosPorTecnicoAsync(
            int idTecnico,
            int pageNumber,
            int pageSize)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.ObterChamadosPorTecnicoAsync");
            (pageNumber, pageSize) = Ajustar(pageNumber, pageSize);
            return await _chamadoRepository.ObterPorTecnicoAsync(idTecnico, pageNumber, pageSize);
        }

        public async Task<Chamado> AdicionarChamadoAsync(ChamadoRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.AdicionarChamadoAsync");

            _logger.LogInformation(
                "Iniciando criacao do chamado {ChamadoId} para usuario {UsuarioId}",
                model.IdChamado,
                model.IdUsuario);

            var usuario = await _usuarioRepository.ObterUmAsync(model.IdUsuario);

            if (usuario is null)
                throw new InvalidOperationException("Usuario nao encontrado.");

            if (!usuario.Ativo)
                throw new InvalidOperationException("Usuario inativo nao pode abrir chamado.");

            ValidarPrioridade(model.Prioridade);
            await ValidarTecnicoAsync(model.IdTecnico);

            var entity = model.ToChamadoEntity();
            entity.Status = "Aberto";
            entity.DataAbertura = DateTime.UtcNow;
            entity.DataAtualizacao = DateTime.UtcNow;
            entity.DataFechamento = null;

            var result = await _chamadoRepository.AdicionarAsync(entity);

            _logger.LogInformation(
                "Chamado {ChamadoId} criado com status {Status}",
                result.IdChamado,
                result.Status);

            return result;
        }

        public async Task<Chamado?> EditarChamadoAsync(int id, ChamadoRequestDto model)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.EditarChamadoAsync");
            var entity = await _chamadoRepository.ObterUmAsync(id);

            if (entity is null) return null;

            if (string.Equals(entity.Status, "Fechado", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Chamado fechado nao pode ser alterado.");

            var usuario = await _usuarioRepository.ObterUmAsync(model.IdUsuario);

            if (usuario is null || !usuario.Ativo)
                throw new InvalidOperationException("Usuario inexistente ou inativo.");

            ValidarPrioridade(model.Prioridade);
            await ValidarTecnicoAsync(model.IdTecnico);

            var statusAnterior = entity.Status;
            var novoStatus = string.IsNullOrWhiteSpace(model.Status)
                ? statusAnterior
                : NormalizarStatus(model.Status);

            ValidarTransicao(statusAnterior, novoStatus);

            model.MapToExisting(entity);
            entity.Status = novoStatus;
            entity.DataAtualizacao = DateTime.UtcNow;
            entity.DataFechamento = novoStatus == "Fechado" ? DateTime.UtcNow : null;

            _logger.LogInformation(
                "Atualizando chamado {ChamadoId} de {StatusAnterior} para {NovoStatus}",
                id,
                statusAnterior,
                novoStatus);

            return await _chamadoRepository.EditarAsync(id, entity);
        }

        public async Task<Chamado?> DeletarChamadoAsync(int id)
        {
            using var activity = ActivitySource.StartActivity("ChamadoUseCase.DeletarChamadoAsync");
            return await _chamadoRepository.DeletarAsync(id);
        }

        private async Task ValidarTecnicoAsync(int? id)
        {
            if (!id.HasValue) return;

            var tecnico = await _tecnicoRepository.ObterUmAsync(id.Value);

            if (tecnico is null)
                throw new InvalidOperationException("Tecnico nao encontrado.");

            if (!tecnico.Ativo)
                throw new InvalidOperationException("Tecnico inativo nao pode receber chamado.");
        }

        private static void ValidarPrioridade(string prioridade)
        {
            if (!PrioridadesValidas.Any(x =>
                string.Equals(x, prioridade, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(
                    "Prioridade invalida. Utilize: Baixa, Media, Alta ou Critica.");
            }
        }

        private static string NormalizarStatus(string status)
        {
            var statusNormalizado = StatusValidos.FirstOrDefault(x =>
                string.Equals(x, status, StringComparison.OrdinalIgnoreCase));

            if (statusNormalizado is null)
            {
                throw new InvalidOperationException(
                    "Status invalido. Utilize: Aberto, Em Atendimento, Resolvido ou Fechado.");
            }

            return statusNormalizado;
        }

        private static void ValidarTransicao(string statusAtual, string novoStatus)
        {
            if (statusAtual == novoStatus) return;

            var transicaoValida = statusAtual switch
            {
                "Aberto" => novoStatus == "Em Atendimento",
                "Em Atendimento" => novoStatus == "Resolvido",
                "Resolvido" => novoStatus == "Fechado",
                _ => false
            };

            if (!transicaoValida)
            {
                throw new InvalidOperationException(
                    $"Transicao de status invalida: {statusAtual} -> {novoStatus}.");
            }
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
