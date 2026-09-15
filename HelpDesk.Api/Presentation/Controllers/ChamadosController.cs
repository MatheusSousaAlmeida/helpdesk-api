using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Doc.Samples;
using HelpDesk.API.Domain.Entities;
using HelpDesk.API.Infrastructure.Observability;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadosController : ControllerBase
    {
        private readonly IChamadoUseCase _chamadoUseCase;
        private readonly ApiMetrics _metrics;
        private readonly ILogger<ChamadosController> _logger;

        public ChamadosController(IChamadoUseCase chamadoUseCase, ApiMetrics metrics, ILogger<ChamadosController> logger)
        {
            _chamadoUseCase = chamadoUseCase;
            _metrics = metrics;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista chamados",
            Description = """
            ## Informacoes do Retorno:
            * **Status 200 (OK):** Retorna os chamados encontrados.
            * **Status 204 (No Content):** Nao existem chamados para a pagina informada.
            * **Status 400 (Bad Request):** Ocorreu uma falha durante a consulta.
            * **Status 429 (Too Many Requests):** O limite de requisicoes foi excedido.

            ## Paginacao:
            * **pageNumber:** Numero da pagina, iniciando em 1.
            * **pageSize:** Quantidade de registros por pagina, limitada a 100.
            """)]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamados retornados com sucesso", typeof(IEnumerable<Chamado>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum chamado encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar os dados", typeof(string))]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Limite de requisicoes excedido")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChamadoResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Listando chamados. Pagina {PageNumber}, tamanho {PageSize}", pageNumber, pageSize);
                var resultado = await _chamadoUseCase.ObterTodosChamadosAsync(pageNumber, pageSize);
                if (!resultado.Any())
                    return NoContent();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar chamados");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca chamado por ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamado encontrado", typeof(Chamado))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Chamado nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar o chamado", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChamadoResponseSample))]
        public async Task<IActionResult> GetById([FromRoute, SwaggerParameter("Id do chamado")] int id)
        {
            _logger.LogInformation("Obtendo chamado com id {ChamadoId}", id);
            try
            {
                var chamado = await _chamadoUseCase.ObterUmChamadoAsync(id);
                if (chamado is null)
                {
                    _logger.LogWarning("Chamado com id {ChamadoId} nao encontrado", id);
                    return NotFound();
                }
                return Ok(chamado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter chamado {ChamadoId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("usuario/{idUsuario:int}")]
        [SwaggerOperation(Summary = "Lista chamados de um usuario")]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamados do usuario retornados", typeof(IEnumerable<Chamado>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum chamado encontrado para o usuario")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChamadoResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetByUsuario(
            [FromRoute, SwaggerParameter("Id do usuario")] int idUsuario,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var resultado = await _chamadoUseCase.ObterChamadosPorUsuarioAsync(idUsuario, pageNumber, pageSize);
                return resultado.Any() ? Ok(resultado) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar chamados do usuario {UsuarioId}", idUsuario);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tecnico/{idTecnico:int}")]
        [SwaggerOperation(Summary = "Lista chamados de um tecnico")]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamados do tecnico retornados", typeof(IEnumerable<Chamado>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum chamado encontrado para o tecnico")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChamadoResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetByTecnico(
            [FromRoute, SwaggerParameter("Id do tecnico")] int idTecnico,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var resultado = await _chamadoUseCase.ObterChamadosPorTecnicoAsync(idTecnico, pageNumber, pageSize);
                return resultado.Any() ? Ok(resultado) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar chamados do tecnico {TecnicoId}", idTecnico);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Abre chamado",
            Description = "O status e definido automaticamente como Aberto e a data de abertura e controlada pela API.")]
        [SwaggerRequestExample(typeof(ChamadoRequestDto), typeof(ChamadoRequestSample))]
        [SwaggerResponse(StatusCodes.Status201Created, "Chamado criado", typeof(Chamado))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Regra de negocio ou dados invalidos", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ChamadoResponseSample))]
        public async Task<IActionResult> Post([FromBody] ChamadoRequestDto model)
        {
            try
            {
                _logger.LogInformation("Iniciando criacao do chamado {ChamadoId} para usuario {UsuarioId}", model.IdChamado, model.IdUsuario);
                var chamado = await _chamadoUseCase.AdicionarChamadoAsync(model);
                _metrics.RecordChamadoCriado();
                return CreatedAtAction(nameof(GetById), new { id = chamado.IdChamado }, chamado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar chamado {ChamadoId}", model.IdChamado);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(
            Summary = "Atualiza chamado",
            Description = "Aplica as regras de transicao Aberto -> Em Atendimento -> Resolvido -> Fechado.")]
        [SwaggerRequestExample(typeof(ChamadoRequestDto), typeof(ChamadoRequestSample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamado atualizado", typeof(Chamado))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Chamado nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Transicao ou regra de negocio invalida", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ChamadoResponseSample))]
        public async Task<IActionResult> Put(
            [FromRoute, SwaggerParameter("Id do chamado")] int id,
            [FromBody] ChamadoRequestDto model)
        {
            try
            {
                var chamado = await _chamadoUseCase.EditarChamadoAsync(id, model);
                return chamado is null ? NotFound() : Ok(chamado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar chamado {ChamadoId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Exclui chamado")]
        [SwaggerResponse(StatusCodes.Status200OK, "Chamado excluido", typeof(Chamado))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Chamado nao encontrado")]
        public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do chamado")] int id)
        {
            try
            {
                var chamado = await _chamadoUseCase.DeletarChamadoAsync(id);
                return chamado is null ? NotFound() : Ok(chamado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir chamado {ChamadoId}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
