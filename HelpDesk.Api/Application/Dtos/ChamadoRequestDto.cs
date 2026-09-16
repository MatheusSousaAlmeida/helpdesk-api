using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.Application.Dtos
{
    public class ChamadoRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O ID do chamado deve ser maior que zero.")]
        public int IdChamado { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do usuario deve ser maior que zero.")]
        public int IdUsuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "O ID do tecnico deve ser maior que zero.")]
        public int? IdTecnico { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 5)]
        public string Descricao { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Prioridade { get; set; } = string.Empty;

        [StringLength(30)]
        public string? Status { get; set; }
    }
}
