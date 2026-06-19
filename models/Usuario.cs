using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PresenteDeDeus.API.Models
{
    public class Usuario
    {
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string SenhaHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;

        public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;

        public bool Ativo { get; set; } = true;

        public DateTime CriadoEm { get; set; } = DateTime.Now;

        public ICollection<Venda> Vendas { get; set; } = new List<Venda>();
    }

    public enum PerfilUsuario
    {
        Admin,
        Operador
    }
}
