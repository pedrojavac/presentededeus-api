using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PresenteDeDeus.API.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Preco { get; set; }

        public int QuantidadeEstoque { get; set; } = 0;

        [MaxLength(80)]
        public string? CodigoBarras { get; set; }

        [MaxLength(100)]
        public string? Categoria { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime CriadoEm { get; set; } = DateTime.Now;
        public DateTime? AtualizadoEm { get; set; } = DateTime.Now;
    }
}
