using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDesk.API.Domain.Entities
{
    [Table("COMENTARIO")]
    [Index(nameof(IdChamado), Name = "IX_COMENTARIO_CHAMADO")]
    [Index(nameof(DataCriacao), Name = "IX_COMENTARIO_DATA_CRIACAO")]
    public class Comentario
    {
        [Key]
        [Column("ID_COMENTARIO")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdComentario { get; set; }

        [Required]
        [Column("ID_CHAMADO")]
        public int IdChamado { get; set; }

        [Required(ErrorMessage = "O autor e obrigatorio.")]
        [StringLength(100)]
        [Column("AUTOR")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "O comentario e obrigatorio.")]
        [StringLength(1000)]
        [Column("TEXTO")]
        public string Texto { get; set; } = string.Empty;

        [Required]
        [Column("DATA_CRIACAO")]
        public DateTime DataCriacao { get; set; }

        public Chamado? Chamado { get; set; }
    }
}
