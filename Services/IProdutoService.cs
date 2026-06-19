using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Models;
using PresenteDeDeus.API.Repositories;

namespace PresenteDeDeus.API.Services
{
    public interface IProdutoService
    {
        Task<List<ProdutoResponseDto>> ListarAsync();
        Task<ProdutoResponseDto?> BuscarPorIdAsync(int id);
        Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto);
        Task<ProdutoResponseDto?> AtualizarAsync(int id, ProdutoRequestDto dto);
        Task<bool> RemoverAsync(int id);
    }

    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repo;

        public ProdutoService(IProdutoRepository repo) => _repo = repo;

        public async Task<List<ProdutoResponseDto>> ListarAsync()
        {
            var produtos = await _repo.ListarTodosAsync();
            return produtos.Select(MapToDto).ToList();
        }

        public async Task<ProdutoResponseDto?> BuscarPorIdAsync(int id)
        {
            var produto = await _repo.BuscarPorIdAsync(id);
            return produto == null ? null : MapToDto(produto);
        }

        public async Task<ProdutoResponseDto> CriarAsync(ProdutoRequestDto dto)
        {
            var produto = new Produto
            {
                Nome = dto.Nome.Trim(),
                Descricao = dto.Descricao?.Trim(),
                Preco = dto.Preco,
                QuantidadeEstoque = dto.QuantidadeEstoque,
                Categoria = dto.Categoria?.Trim(),
                CodigoBarras = dto.CodigoBarras?.Trim()
            };

            var criado = await _repo.AdicionarAsync(produto);
            return MapToDto(criado);
        }

        public async Task<ProdutoResponseDto?> AtualizarAsync(int id, ProdutoRequestDto dto)
        {
            var produto = await _repo.BuscarPorIdAsync(id);
            if (produto == null) return null;

            produto.Nome = dto.Nome.Trim();
            produto.Descricao = dto.Descricao?.Trim();
            produto.Preco = dto.Preco;
            produto.QuantidadeEstoque = dto.QuantidadeEstoque;
            produto.Categoria = dto.Categoria?.Trim();
            produto.CodigoBarras = dto.CodigoBarras?.Trim();
            produto.AtualizadoEm = DateTime.Now;

            var atualizado = await _repo.AtualizarAsync(produto);
            return MapToDto(atualizado);
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var produto = await _repo.BuscarPorIdAsync(id);
            if (produto == null) return false;
            await _repo.RemoverAsync(id);
            return true;
        }

        private static ProdutoResponseDto MapToDto(Produto p) => new()
        {
            Id = p.Id,
            Nome = p.Nome,
            Descricao = p.Descricao,
            Preco = p.Preco,
            QuantidadeEstoque = p.QuantidadeEstoque,
            Categoria = p.Categoria,
            CodigoBarras = p.CodigoBarras,
            Ativo = p.Ativo
        };
    }
}
