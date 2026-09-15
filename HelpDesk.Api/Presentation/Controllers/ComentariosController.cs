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
    public class ComentariosController : ControllerBase
    {
        private readonly IComentarioUseCase _comentarioUseCase;
        private readonly ILogger<ComentariosController> _logger;

        public ComentariosController(IComentarioUseCase comentarioUseCase, ILogger<ComentariosController> logger)
        {
            _comentarioUseCase = comentarioUseCase;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista comentarios",
            Description = """
            ## Informacoes do Retorno:
            * **Status 200 (OK):** Retorna os comentarios encontrados.
            * **Status 204 (No Content):** Nao existem comentarios para a pagina informada.
            * **Status 400 (Bad Request):** Ocorreu uma falha durante a consulta.
            * **Status 429 (Too Many Requests):** O limite de requisicoes foi excedido.

            ## Paginacao:
            * **pageNumber:** Numero da pagina, iniciando em 1.
            * **pageSize:** Quantidade de registros por pagina, limitada a 100.
            """)]
        [SwaggerResponse(StatusCodes.Status200OK, "Comentarios retornados com sucesso", typeof(IEnumerable<Comentario>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum comentario encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar os dados", typeof(string))]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Limite de requisicoes excedido")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComentarioResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var resultado = await _comentarioUseCase.ObterTodosComentariosAsync(pageNumber, pageSize);
                if (!resultado.Any())
                    return NoContent();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar comentarios");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca comentario por ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comentario encontrado", typeof(Comentario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comentario nao encontrado")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComentarioResponseSample))]
        public async Task<IActionResult> GetById([FromRoute, SwaggerParameter("Id do comentario")] int id)
        {
            try
            {
                var comentario = await _comentarioUseCase.ObterUmComentarioAsync(id);
                return comentario is null ? NotFound() : Ok(comentario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter comentario {ComentarioId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("chamado/{idChamado:int}")]
        [SwaggerOperation(Summary = "Lista comentarios de um chamado")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comentarios do chamado retornados", typeof(IEnumerable<Comentario>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum comentario encontrado para o chamado")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComentarioResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetByChamado(
            [FromRoute, SwaggerParameter("Id do chamado")] int idChamado,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var resultado = await _comentarioUseCase.ObterComentariosPorChamadoAsync(idChamado, pageNumber, pageSize);
                return resultado.Any() ? Ok(resultado) : NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar comentarios do chamado {ChamadoId}", idChamado);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Adiciona comentario", Description = "Chamados fechados nao aceitam novos comentarios.")]
        [SwaggerRequestExample(typeof(ComentarioRequestDto), typeof(ComentarioRequestSample))]
        [SwaggerResponse(StatusCodes.Status201Created, "Comentario criado", typeof(Comentario))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Regra de negocio invalida", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(ComentarioResponseSample))]
        public async Task<IActionResult> Post([FromBody] ComentarioRequestDto model)
        {
            try
            {
                var comentario = await _comentarioUseCase.AdicionarComentarioAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = comentario.IdComentario }, comentario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar comentario {ComentarioId}", model.IdComentario);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza comentario")]
        [SwaggerRequestExample(typeof(ComentarioRequestDto), typeof(ComentarioRequestSample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Comentario atualizado", typeof(Comentario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comentario nao encontrado")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ComentarioResponseSample))]
        public async Task<IActionResult> Put(
            [FromRoute, SwaggerParameter("Id do comentario")] int id,
            [FromBody] ComentarioRequestDto model)
        {
            try
            {
                var comentario = await _comentarioUseCase.EditarComentarioAsync(id, model);
                return comentario is null ? NotFound() : Ok(comentario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar comentario {ComentarioId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Exclui comentario")]
        [SwaggerResponse(StatusCodes.Status200OK, "Comentario excluido", typeof(Comentario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Comentario nao encontrado")]
        public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do comentario")] int id)
        {
            try
            {
                var comentario = await _comentarioUseCase.DeletarComentarioAsync(id);
                return comentario is null ? NotFound() : Ok(comentario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir comentario {ComentarioId}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
