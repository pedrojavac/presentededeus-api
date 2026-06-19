using System.ComponentModel.DataAnnotations;

namespace PresenteDeDeus.API.DTOs
{
    public class ProdutoRequestDto
    {
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        [MaxLength(150)]
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        public int QuantidadeEstoque { get; set; }

        public string? Categoria { get; set; }
        public string? CodigoBarras { get; set; }
    }

    public class ProdutoResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public int QuantidadeEstoque { get; set; }
        public string? Categoria { get; set; }
        public string? CodigoBarras { get; set; }
        public bool Ativo { get; set; }
        public bool EstoqueAlerta => QuantidadeEstoque <= 5; // Propriedade calculada
    }
}

