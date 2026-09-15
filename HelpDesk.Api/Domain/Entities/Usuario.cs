using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HelpDesk.API.Domain.Entities
{
    [Table("USUARIO")]
    [Index(nameof(Email), IsUnique = true, Name = "UX_USUARIO_EMAIL")]
    public class Usuario
    {
        [Key]
        [Column("ID_USUARIO")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "O nome e obrigatorio.")]
        [StringLength(100)]
        [Column("NOME")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail e obrigatorio.")]
        [EmailAddress(ErrorMessage = "E-mail em formato invalido.")]
        [StringLength(100)]
        [Column("EMAIL")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O departamento e obrigatorio.")]
        [StringLength(100)]
        [Column("DEPARTAMENTO")]
        public string Departamento { get; set; } = string.Empty;

        [Required]
        [Column("ATIVO")]
        public bool Ativo { get; set; }

        [JsonIgnore]
        public ICollection<Chamado>? Chamados { get; set; }
    }
}
