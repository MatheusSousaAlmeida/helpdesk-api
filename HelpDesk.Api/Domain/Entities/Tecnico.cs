using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HelpDesk.API.Domain.Entities
{
    [Table("TECNICO")]
    [Index(nameof(Email), IsUnique = true, Name = "UX_TECNICO_EMAIL")]
    [Index(nameof(Especialidade), Name = "IX_TECNICO_ESPECIALIDADE")]
    public class Tecnico
    {
        [Key]
        [Column("ID_TECNICO")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdTecnico { get; set; }

        [Required(ErrorMessage = "O nome e obrigatorio.")]
        [StringLength(100)]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail e obrigatorio.")]
        [EmailAddress(ErrorMessage = "E-mail em formato invalido.")]
        [StringLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A especialidade e obrigatoria.")]
        [StringLength(50)]
        [Column("ESPECIALIDADE")]
        public string Especialidade { get; set; } = string.Empty;

        [Required]
        [Column("ATIVO")]
        public bool Ativo { get; set; }

        [JsonIgnore]
        public ICollection<Chamado>? Chamados { get; set; }
    }
}
