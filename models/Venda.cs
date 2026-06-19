using System.ComponentModel.DataAnnotations.Schema;

namespace PresenteDeDeus.API.Models
{
    public class Venda
    {
        public int Id { get; set; }

        public DateTime DataHora { get; set; } = DateTime.Now;

        public int OperadorId { get; set; }
        public Usuario? Operador { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalVenda { get; set; }

        public FormaPagamento FormaPagamento { get; set; }

        public string? Observacao { get; set; }

        public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
    }

    public enum FormaPagamento
    {
        Dinheiro,
        PIX,
        Cartao_Debito,
        Cartao_Credito
    }
}
