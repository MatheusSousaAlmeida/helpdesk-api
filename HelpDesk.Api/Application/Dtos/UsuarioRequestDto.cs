using System.ComponentModel.DataAnnotations;
namespace HelpDesk.API.Application.Dtos
{
 public class UsuarioRequestDto
 {
  [Range(1,int.MaxValue,ErrorMessage="O ID do usuario deve ser maior que zero.")] public int IdUsuario { get; set; }
  [Required][StringLength(100,MinimumLength=2)] public string Nome { get; set; } = string.Empty;
  [Required][EmailAddress][StringLength(100)] public string Email { get; set; } = string.Empty;
  [Required][StringLength(100)] public string Departamento { get; set; } = string.Empty;
  public bool Ativo { get; set; } = true;
 }
}
