using HelpDesk.API.Application.Dtos;
using HelpDesk.API.Application.Interfaces;
using HelpDesk.API.Doc.Samples;
using HelpDesk.API.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace HelpDesk.API.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TecnicosController : ControllerBase
    {
        private readonly ITecnicoUseCase _tecnicoUseCase;
        private readonly ILogger<TecnicosController> _logger;

        public TecnicosController(ITecnicoUseCase tecnicoUseCase, ILogger<TecnicosController> logger)
        {
            _tecnicoUseCase = tecnicoUseCase;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista tecnicos",
            Description = """
            ## Informacoes do Retorno:
            * **Status 200 (OK):** Retorna os tecnicos encontrados.
            * **Status 204 (No Content):** Nao existem tecnicos para a pagina informada.
            * **Status 400 (Bad Request):** Ocorreu uma falha durante a consulta.
            * **Status 429 (Too Many Requests):** O limite de requisicoes foi excedido.

            ## Paginacao:
            * **pageNumber:** Numero da pagina, iniciando em 1.
            * **pageSize:** Quantidade de registros por pagina, limitada a 100.
            """)]
        [SwaggerResponse(StatusCodes.Status200OK, "Tecnicos retornados com sucesso", typeof(IEnumerable<Tecnico>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum tecnico encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar os dados", typeof(string))]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Limite de requisicoes excedido")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TecnicoResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Listando tecnicos. Pagina {PageNumber}, tamanho {PageSize}", pageNumber, pageSize);
                var resultado = await _tecnicoUseCase.ObterTodosTecnicosAsync(pageNumber, pageSize);
                if (!resultado.Any())
                    return NoContent();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar tecnicos");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca tecnico por ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tecnico encontrado", typeof(Tecnico))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tecnico nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar o tecnico", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TecnicoResponseSample))]
        public async Task<IActionResult> GetById([FromRoute, SwaggerParameter("Id do tecnico")] int id)
        {
            try
            {
                _logger.LogInformation("Obtendo tecnico com id {TecnicoId}", id);
                var tecnico = await _tecnicoUseCase.ObterUmTecnicoAsync(id);
                if (tecnico is null)
                {
                    _logger.LogWarning("Tecnico com id {TecnicoId} nao encontrado", id);
                    return NotFound();
                }
                return Ok(tecnico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tecnico {TecnicoId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra tecnico")]
        [SwaggerRequestExample(typeof(TecnicoRequestDto), typeof(TecnicoRequestSample))]
        [SwaggerResponse(StatusCodes.Status201Created, "Tecnico cadastrado", typeof(Tecnico))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados invalidos ou e-mail duplicado", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(TecnicoResponseSample))]
        public async Task<IActionResult> Post([FromBody] TecnicoRequestDto model)
        {
            try
            {
                var tecnico = await _tecnicoUseCase.AdicionarTecnicoAsync(model);
                if (tecnico is null)
                    return BadRequest("E-mail ja cadastrado.");

                return CreatedAtAction(nameof(GetById), new { id = tecnico.IdTecnico }, tecnico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar tecnico {TecnicoId}", model.IdTecnico);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza tecnico")]
        [SwaggerRequestExample(typeof(TecnicoRequestDto), typeof(TecnicoRequestSample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Tecnico atualizado", typeof(Tecnico))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tecnico nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados invalidos", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(TecnicoResponseSample))]
        public async Task<IActionResult> Put(
            [FromRoute, SwaggerParameter("Id do tecnico")] int id,
            [FromBody] TecnicoRequestDto model)
        {
            try
            {
                var tecnico = await _tecnicoUseCase.EditarTecnicoAsync(id, model);
                return tecnico is null ? NotFound() : Ok(tecnico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tecnico {TecnicoId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Exclui tecnico")]
        [SwaggerResponse(StatusCodes.Status200OK, "Tecnico excluido", typeof(Tecnico))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Tecnico nao encontrado")]
        public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do tecnico")] int id)
        {
            try
            {
                var tecnico = await _tecnicoUseCase.DeletarTecnicoAsync(id);
                return tecnico is null ? NotFound() : Ok(tecnico);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tecnico {TecnicoId}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
