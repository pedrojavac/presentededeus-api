using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using PresenteDeDeus.API.Data;
using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Models;
using PresenteDeDeus.API.Repositories;

namespace PresenteDeDeus.API.Services
{
    public interface IVendaService
    {
        Task<VendaResponseDto> RegistrarVendaAsync(NovaVendaDto dto, ClaimsPrincipal operador);
        Task<List<VendaResponseDto>> ListarVendasAsync(ClaimsPrincipal operador);
        Task<VendaResponseDto?> BuscarPorIdAsync(int id, ClaimsPrincipal operador);
        Task<bool> RemoverVendaAsync(int id, ClaimsPrincipal operador);
    }

    public class VendaService : IVendaService
    {
        private readonly AppDbContext _context;
        private readonly IProdutoRepository _produtoRepo;
        private readonly IVendaRepository _vendaRepo;

        public VendaService(
            AppDbContext context,
            IProdutoRepository produtoRepo,
            IVendaRepository vendaRepo)
        {
            _context = context;
            _produtoRepo = produtoRepo;
            _vendaRepo = vendaRepo;
        }

        public async Task<VendaResponseDto> RegistrarVendaAsync(
            NovaVendaDto dto, ClaimsPrincipal operador)
        {
            var operadorId = int.Parse(
                operador.FindFirst("UsuarioId")?.Value
                ?? throw new UnauthorizedAccessException("Token inválido."));

            if (!Enum.TryParse<FormaPagamento>(dto.FormaPagamento, ignoreCase: true, out var formaPagamento))
                throw new ArgumentException(
                    $"Forma de pagamento inválida: '{dto.FormaPagamento}'.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var venda = new Venda
                {
                    OperadorId = operadorId,
                    FormaPagamento = formaPagamento,
                    Observacao = dto.Observacao,
                    DataHora = DateTime.Now
                };

                decimal total = 0;

                foreach (var itemDto in dto.Itens)
                {
                    var produto = await _produtoRepo.BuscarPorIdAsync(itemDto.ProdutoId)
                        ?? throw new KeyNotFoundException(
                            $"Produto {itemDto.ProdutoId} não encontrado.");

                    if (!produto.Ativo)
                        throw new InvalidOperationException(
                            $"Produto '{produto.Nome}' está inativo e não pode ser vendido.");

                    if (produto.QuantidadeEstoque < itemDto.Quantidade)
                        throw new InvalidOperationException(
                            $"Estoque insuficiente para '{produto.Nome}'. " +
                            $"Disponível: {produto.QuantidadeEstoque}.");

                    var subtotal = produto.Preco * itemDto.Quantidade;
                    total += subtotal;

                    venda.Itens.Add(new ItemVenda
                    {
                        ProdutoId = produto.Id,
                        Quantidade = itemDto.Quantidade,
                        PrecoUnitario = produto.Preco,
                        Subtotal = subtotal
                    });

                    await _produtoRepo.AtualizarEstoqueAsync(
                        produto.Id, -itemDto.Quantidade);
                }

                venda.TotalVenda = total;
                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                var vendaCompleta = await _vendaRepo.BuscarPorIdAsync(venda.Id);
                return MapVendaDto(vendaCompleta!);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<List<VendaResponseDto>> ListarVendasAsync(ClaimsPrincipal operador)
        {
            var isAdmin = operador.IsInRole("Admin");
            var operadorId = int.Parse(operador.FindFirst("UsuarioId")!.Value);

            var vendas = isAdmin
                ? await _vendaRepo.ListarTodasAsync()
                : await _vendaRepo.ListarPorOperadorAsync(operadorId);

            return vendas.Select(MapVendaDto).ToList();
        }

        public async Task<VendaResponseDto?> BuscarPorIdAsync(int id, ClaimsPrincipal operador)
        {
            var isAdmin = operador.IsInRole("Admin");
            var operadorId = int.Parse(operador.FindFirst("UsuarioId")!.Value);

            var venda = await _vendaRepo.BuscarPorIdAsync(id);

            if (venda == null) return null;

            if (!isAdmin && venda.OperadorId != operadorId)
                throw new UnauthorizedAccessException("Acesso negado.");

            return MapVendaDto(venda);
        }

        // Remove definitivamente a venda do banco e devolve os itens ao estoque.
        // Apenas administradores podem excluir vendas.
        public async Task<bool> RemoverVendaAsync(int id, ClaimsPrincipal operador)
        {
            var isAdmin = operador.IsInRole("Admin");
            if (!isAdmin)
                throw new UnauthorizedAccessException(
                    "Apenas administradores podem excluir vendas.");

            var venda = await _vendaRepo.BuscarPorIdAsync(id);
            if (venda == null) return false;

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // Devolve a quantidade de cada item ao estoque do produto
                foreach (var item in venda.Itens)
                {
                    await _produtoRepo.AtualizarEstoqueAsync(
                        item.ProdutoId, item.Quantidade);
                }

                await _vendaRepo.RemoverAsync(venda);
                await tx.CommitAsync();
                return true;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private static VendaResponseDto MapVendaDto(Venda v) => new()
        {
            Id = v.Id,
            DataHora = v.DataHora,
            Operador = v.Operador?.NomeCompleto ?? "Desconhecido",
            TotalVenda = v.TotalVenda,
            FormaPagamento = v.FormaPagamento.ToString(),
            Observacao = v.Observacao,
            Itens = v.Itens.Select(i => new ItemVendaResponseDto
            {
                NomeProduto = i.Produto?.Nome ?? "Produto removido",
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}


