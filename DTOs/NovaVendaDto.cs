using System.ComponentModel.DataAnnotations;

namespace PresenteDeDeus.API.DTOs
{
    // DTO recebido do front-end ao registrar uma nova venda
    public class NovaVendaDto
    {
        [Required]
        public string FormaPagamento { get; set; } = string.Empty;

        public string? Observacao { get; set; }

        [MinLength(1, ErrorMessage = "A venda deve ter ao menos 1 item.")]
        public List<ItemVendaDto> Itens { get; set; } = new();
    }

    // Cada produto dentro da venda
    public class ItemVendaDto
    {
        public int ProdutoId { get; set; }

        [Range(1, 9999, ErrorMessage = "Quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    // DTO de resposta da venda registrada (enviado ao front-end)
    public class VendaResponseDto
    {
        public int Id { get; set; }
        public DateTime DataHora { get; set; }
        public string Operador { get; set; } = string.Empty;
        public decimal TotalVenda { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public string? Observacao { get; set; }
        public List<ItemVendaResponseDto> Itens { get; set; } = new();
    }

    public class ItemVendaResponseDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
