using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HelpDesk.API.Domain.Entities
{
    [Table("CHAMADO")]
    [Index(nameof(Status), Name = "IX_CHAMADO_STATUS")]
    [Index(nameof(Prioridade), Name = "IX_CHAMADO_PRIORIDADE")]
    [Index(nameof(IdUsuario), Name = "IX_CHAMADO_USUARIO")]
    [Index(nameof(IdTecnico), Name = "IX_CHAMADO_TECNICO")]
    [Index(nameof(DataAbertura), Name = "IX_CHAMADO_DATA_ABERTURA")]
    [Index(nameof(Status), nameof(Prioridade), Name = "IX_CHAMADO_STATUS_PRIORIDADE")]
    public class Chamado
    {
        [Key]
        [Column("ID_CHAMADO")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdChamado { get; set; }

        [Required]
        [Column("ID_USUARIO")]
        public int IdUsuario { get; set; }

        [Column("ID_TECNICO")]
        public int? IdTecnico { get; set; }

        [Required(ErrorMessage = "O titulo e obrigatorio.")]
        [StringLength(150)]
        [Column("TITULO")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descricao e obrigatoria.")]
        [StringLength(1000)]
        [Column("DESCRICAO")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "A prioridade e obrigatoria.")]
        [StringLength(20)]
        [Column("PRIORIDADE")]
        public string Prioridade { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status e obrigatorio.")]
        [StringLength(30)]
        [Column("STATUS")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [Column("DATA_ABERTURA")]
        public DateTime DataAbertura { get; set; }

        [Column("DATA_ATUALIZACAO")]
        public DateTime? DataAtualizacao { get; set; }

        [Column("DATA_FECHAMENTO")]
        public DateTime? DataFechamento { get; set; }

        public Usuario? Usuario { get; set; }
        public Tecnico? Tecnico { get; set; }

        [JsonIgnore]
        public ICollection<Comentario>? Comentarios { get; set; }
    }
}
