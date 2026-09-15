using System.ComponentModel.DataAnnotations;
namespace HelpDesk.API.Application.Dtos
{
 public class ComentarioRequestDto
 {
  [Range(1,int.MaxValue,ErrorMessage="O ID do comentario deve ser maior que zero.")] public int IdComentario { get; set; }
  [Range(1,int.MaxValue,ErrorMessage="O ID do chamado deve ser maior que zero.")] public int IdChamado { get; set; }
  [Required][StringLength(100,MinimumLength=2)] public string Autor { get; set; } = string.Empty;
  [Required][StringLength(1000,MinimumLength=1)] public string Texto { get; set; } = string.Empty;
 }
}
