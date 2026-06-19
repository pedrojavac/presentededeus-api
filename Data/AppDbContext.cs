using Microsoft.EntityFrameworkCore;
using PresenteDeDeus.API.Models;

namespace PresenteDeDeus.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Login)
                .IsUnique();

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Operador)
                .WithMany(u => u.Vendas)
                .HasForeignKey(v => v.OperadorId);

            modelBuilder.Entity<ItemVenda>()
                .HasOne(i => i.Venda)
                .WithMany(v => v.Itens)
                .HasForeignKey(i => i.VendaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Perfil)
                .HasConversion<string>();

            modelBuilder.Entity<Venda>()
                .Property(v => v.FormaPagamento)
                .HasConversion<string>();
        }
    }
}
