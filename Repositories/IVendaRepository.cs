using Microsoft.EntityFrameworkCore;
using PresenteDeDeus.API.Data;
using PresenteDeDeus.API.Models;

namespace PresenteDeDeus.API.Repositories
{
    public interface IVendaRepository
    {
        Task<List<Venda>> ListarPorOperadorAsync(int operadorId);
        Task<List<Venda>> ListarTodasAsync();
        Task<Venda?> BuscarPorIdAsync(int id);
    }

    public class VendaRepository : IVendaRepository
    {
        private readonly AppDbContext _context;

        public VendaRepository(AppDbContext context) => _context = context;

        // Admin: lista todas as vendas com JOIN nas tabelas relacionadas
        public async Task<List<Venda>> ListarTodasAsync()
            => await _context.Vendas
                .Include(v => v.Operador)             // JOIN Usuarios
                .Include(v => v.Itens)                // JOIN ItensVenda
                    .ThenInclude(i => i.Produto)      // JOIN Produtos
                .OrderByDescending(v => v.DataHora)   // Mais recentes primeiro
                .ToListAsync();

        // Operador: lista apenas as vendas do próprio operador
        public async Task<List<Venda>> ListarPorOperadorAsync(int operadorId)
            => await _context.Vendas
                .Include(v => v.Operador)
                .Include(v => v.Itens).ThenInclude(i => i.Produto)
                .Where(v => v.OperadorId == operadorId)
                .OrderByDescending(v => v.DataHora)
                .ToListAsync();

        // Busca uma venda específica pelo ID com todos os detalhes
        public async Task<Venda?> BuscarPorIdAsync(int id)
            => await _context.Vendas
                .Include(v => v.Operador)
                .Include(v => v.Itens).ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);
    }
}

