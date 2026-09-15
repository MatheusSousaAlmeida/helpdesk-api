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
  private static readonly ActivitySource ActivitySource=new("HelpDesk.Application");
  private static readonly string[] PrioridadesValidas=["Baixa","Media","Alta","Critica"];
  private static readonly string[] StatusValidos=["Aberto","Em Atendimento","Resolvido","Fechado"];
  private readonly IChamadoRepository _chamadoRepository; private readonly IUsuarioRepository _usuarioRepository; private readonly ITecnicoRepository _tecnicoRepository; private readonly ILogger<ChamadoUseCase> _logger;
  public ChamadoUseCase(IChamadoRepository chamadoRepository,IUsuarioRepository usuarioRepository,ITecnicoRepository tecnicoRepository,ILogger<ChamadoUseCase> logger){_chamadoRepository=chamadoRepository;_usuarioRepository=usuarioRepository;_tecnicoRepository=tecnicoRepository;_logger=logger;}
  public async Task<IEnumerable<Chamado>> ObterTodosChamadosAsync(int pageNumber,int pageSize){using var a=ActivitySource.StartActivity("ChamadoUseCase.ObterTodosChamadosAsync");(pageNumber,pageSize)=Ajustar(pageNumber,pageSize);_logger.LogInformation("Obtendo chamados pagina {PageNumber} com tamanho {PageSize}",pageNumber,pageSize);return await _chamadoRepository.ObterTodosAsync(pageNumber,pageSize);}
  public async Task<Chamado?> ObterUmChamadoAsync(int id){using var a=ActivitySource.StartActivity("ChamadoUseCase.ObterUmChamadoAsync");a?.SetTag("chamado.id",id);_logger.LogInformation("Obtendo chamado {ChamadoId} do repository",id);var e=await _chamadoRepository.ObterUmAsync(id);if(e is null)_logger.LogWarning("Chamado {ChamadoId} nao encontrado",id);return e;}
  public async Task<IEnumerable<Chamado>> ObterChamadosPorUsuarioAsync(int idUsuario,int pageNumber,int pageSize){using var a=ActivitySource.StartActivity("ChamadoUseCase.ObterChamadosPorUsuarioAsync");(pageNumber,pageSize)=Ajustar(pageNumber,pageSize);return await _chamadoRepository.ObterPorUsuarioAsync(idUsuario,pageNumber,pageSize);}
  public async Task<IEnumerable<Chamado>> ObterChamadosPorTecnicoAsync(int idTecnico,int pageNumber,int pageSize){using var a=ActivitySource.StartActivity("ChamadoUseCase.ObterChamadosPorTecnicoAsync");(pageNumber,pageSize)=Ajustar(pageNumber,pageSize);return await _chamadoRepository.ObterPorTecnicoAsync(idTecnico,pageNumber,pageSize);}
  public async Task<Chamado> AdicionarChamadoAsync(ChamadoRequestDto model)
  {
   using var a=ActivitySource.StartActivity("ChamadoUseCase.AdicionarChamadoAsync");_logger.LogInformation("Iniciando criacao do chamado {ChamadoId} para usuario {UsuarioId}",model.IdChamado,model.IdUsuario);
   var u=await _usuarioRepository.ObterUmAsync(model.IdUsuario);if(u is null)throw new InvalidOperationException("Usuario nao encontrado.");if(!u.Ativo)throw new InvalidOperationException("Usuario inativo nao pode abrir chamado.");
   ValidarPrioridade(model.Prioridade);await ValidarTecnicoAsync(model.IdTecnico);
   var e=model.ToChamadoEntity();e.Status="Aberto";e.DataAbertura=DateTime.UtcNow;e.DataAtualizacao=DateTime.UtcNow;e.DataFechamento=null;
   var r=await _chamadoRepository.AdicionarAsync(e);_logger.LogInformation("Chamado {ChamadoId} criado com status {Status}",r.IdChamado,r.Status);return r;
  }
  public async Task<Chamado?> EditarChamadoAsync(int id,ChamadoRequestDto model)
  {
   using var a=ActivitySource.StartActivity("ChamadoUseCase.EditarChamadoAsync");var e=await _chamadoRepository.ObterUmAsync(id);if(e is null)return null;if(string.Equals(e.Status,"Fechado",StringComparison.OrdinalIgnoreCase))throw new InvalidOperationException("Chamado fechado nao pode ser alterado.");
   var u=await _usuarioRepository.ObterUmAsync(model.IdUsuario);if(u is null||!u.Ativo)throw new InvalidOperationException("Usuario inexistente ou inativo.");ValidarPrioridade(model.Prioridade);await ValidarTecnicoAsync(model.IdTecnico);
   var anterior=e.Status;var novo=string.IsNullOrWhiteSpace(model.Status)?anterior:NormalizarStatus(model.Status);ValidarTransicao(anterior,novo);model.MapToExisting(e);e.Status=novo;e.DataAtualizacao=DateTime.UtcNow;e.DataFechamento=novo=="Fechado"?DateTime.UtcNow:null;_logger.LogInformation("Atualizando chamado {ChamadoId} de {StatusAnterior} para {NovoStatus}",id,anterior,novo);return await _chamadoRepository.EditarAsync(id,e);
  }
  public async Task<Chamado?> DeletarChamadoAsync(int id){using var a=ActivitySource.StartActivity("ChamadoUseCase.DeletarChamadoAsync");return await _chamadoRepository.DeletarAsync(id);}
  private async Task ValidarTecnicoAsync(int? id){if(!id.HasValue)return;var t=await _tecnicoRepository.ObterUmAsync(id.Value);if(t is null)throw new InvalidOperationException("Tecnico nao encontrado.");if(!t.Ativo)throw new InvalidOperationException("Tecnico inativo nao pode receber chamado.");}
  private static void ValidarPrioridade(string p){if(!PrioridadesValidas.Any(x=>string.Equals(x,p,StringComparison.OrdinalIgnoreCase)))throw new InvalidOperationException("Prioridade invalida. Utilize: Baixa, Media, Alta ou Critica.");}
  private static string NormalizarStatus(string s){var x=StatusValidos.FirstOrDefault(v=>string.Equals(v,s,StringComparison.OrdinalIgnoreCase));if(x is null)throw new InvalidOperationException("Status invalido. Utilize: Aberto, Em Atendimento, Resolvido ou Fechado.");return x;}
  private static void ValidarTransicao(string atual,string novo){if(atual==novo)return;var ok=atual switch{"Aberto"=>novo=="Em Atendimento","Em Atendimento"=>novo=="Resolvido","Resolvido"=>novo=="Fechado",_=>false};if(!ok)throw new InvalidOperationException($"Transicao de status invalida: {atual} -> {novo}.");}
  private static (int,int) Ajustar(int p,int s){if(p<=0)p=1;if(s<=0)s=10;if(s>100)s=100;return(p,s);}
 }
}
