using Microsoft.EntityFrameworkCore;
using PresenteDeDeus.API.Data;
using PresenteDeDeus.API.Models;

namespace PresenteDeDeus.API.Repositories
{
    public interface IProdutoRepository
    {
        Task<List<Produto>> ListarTodosAsync();
        Task<Produto?> BuscarPorIdAsync(int id);
        Task<Produto> AdicionarAsync(Produto produto);
        Task<Produto> AtualizarAsync(Produto produto);
        Task RemoverAsync(int id);
        Task AtualizarEstoqueAsync(int produtoId, int quantidade);
    }

    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _context;

        public ProdutoRepository(AppDbContext context) => _context = context;

        public async Task<List<Produto>> ListarTodosAsync()
            => await _context.Produtos
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();

        public async Task<Produto?> BuscarPorIdAsync(int id)
            => await _context.Produtos.FindAsync(id);

        public async Task<Produto> AdicionarAsync(Produto produto)
        {
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task<Produto> AtualizarAsync(Produto produto)
        {
            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return produto;
        }

        public async Task RemoverAsync(int id)
        {
            var produto = await BuscarPorIdAsync(id);
            if (produto == null) return;
            produto.Ativo = false;
            await AtualizarAsync(produto);
        }

        public async Task AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await BuscarPorIdAsync(produtoId)
                ?? throw new KeyNotFoundException(
                    $"Produto ID {produtoId} não encontrado.");

            produto.QuantidadeEstoque += quantidade;

            if (produto.QuantidadeEstoque < 0)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para o produto '{produto.Nome}'.");

            await AtualizarAsync(produto);
        }
    }
}
