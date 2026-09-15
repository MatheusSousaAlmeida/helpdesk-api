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
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioUseCase _usuarioUseCase;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioUseCase usuarioUseCase, ILogger<UsuariosController> logger)
        {
            _usuarioUseCase = usuarioUseCase;
            _logger = logger;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Lista usuarios",
            Description = """
            ## Informacoes do Retorno:
            * **Status 200 (OK):** Retorna os usuarios encontrados.
            * **Status 204 (No Content):** Nao existem usuarios para a pagina informada.
            * **Status 400 (Bad Request):** Ocorreu uma falha durante a consulta.
            * **Status 429 (Too Many Requests):** O limite de requisicoes foi excedido.

            ## Paginacao:
            * **pageNumber:** Numero da pagina, iniciando em 1.
            * **pageSize:** Quantidade de registros por pagina, limitada a 100.
            """)]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuarios retornados com sucesso", typeof(IEnumerable<Usuario>))]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Nenhum usuario encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar os dados", typeof(string))]
        [SwaggerResponse(StatusCodes.Status429TooManyRequests, "Limite de requisicoes excedido")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseListSample))]
        [EnableRateLimiting("politica_5_tentativas")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Listando usuarios. Pagina {PageNumber}, tamanho {PageSize}", pageNumber, pageSize);
            try
            {
                var resultado = await _usuarioUseCase.ObterTodosUsuariosAsync(pageNumber, pageSize);
                if (!resultado.Any())
                    return NoContent();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Busca usuario por ID")]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuario encontrado", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuario nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ocorreu um erro ao retornar o usuario", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseSample))]
        public async Task<IActionResult> GetById([FromRoute, SwaggerParameter("Id do usuario")] int id)
        {
            _logger.LogInformation("Obtendo usuario com id {UsuarioId}", id);
            try
            {
                var usuario = await _usuarioUseCase.ObterUmUsuarioAsync(id);
                if (usuario is null)
                {
                    _logger.LogWarning("Usuario com id {UsuarioId} nao encontrado", id);
                    return NotFound();
                }

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter usuario {UsuarioId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastra usuario")]
        [SwaggerRequestExample(typeof(UsuarioRequestDto), typeof(UsuarioRequestSample))]
        [SwaggerResponse(StatusCodes.Status201Created, "Usuario cadastrado", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados invalidos ou e-mail duplicado", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(UsuarioResponseSample))]
        public async Task<IActionResult> Post([FromBody] UsuarioRequestDto model)
        {
            _logger.LogInformation("Iniciando cadastro do usuario {UsuarioId}", model.IdUsuario);
            try
            {
                var usuario = await _usuarioUseCase.AdicionarUsuarioAsync(model);
                if (usuario is null)
                    return BadRequest("E-mail ja cadastrado.");

                return CreatedAtAction(nameof(GetById), new { id = usuario.IdUsuario }, usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuario {UsuarioId}", model.IdUsuario);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Atualiza usuario")]
        [SwaggerRequestExample(typeof(UsuarioRequestDto), typeof(UsuarioRequestSample))]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuario atualizado", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuario nao encontrado")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Dados invalidos", typeof(string))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(UsuarioResponseSample))]
        public async Task<IActionResult> Put(
            [FromRoute, SwaggerParameter("Id do usuario")] int id,
            [FromBody] UsuarioRequestDto model)
        {
            try
            {
                var usuario = await _usuarioUseCase.EditarUsuarioAsync(id, model);
                return usuario is null ? NotFound() : Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuario {UsuarioId}", id);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Exclui usuario")]
        [SwaggerResponse(StatusCodes.Status200OK, "Usuario excluido", typeof(Usuario))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Usuario nao encontrado")]
        public async Task<IActionResult> Delete([FromRoute, SwaggerParameter("Id do usuario")] int id)
        {
            try
            {
                var usuario = await _usuarioUseCase.DeletarUsuarioAsync(id);
                return usuario is null ? NotFound() : Ok(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir usuario {UsuarioId}", id);
                return BadRequest(ex.Message);
            }
        }
    }
}
