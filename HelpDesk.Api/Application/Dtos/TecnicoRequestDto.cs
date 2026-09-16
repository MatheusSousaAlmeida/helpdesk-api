using System.ComponentModel.DataAnnotations;

namespace HelpDesk.API.Application.Dtos
{
    public class TecnicoRequestDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "O ID do tecnico deve ser maior que zero.")]
        public int IdTecnico { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Especialidade { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;
    }
}
