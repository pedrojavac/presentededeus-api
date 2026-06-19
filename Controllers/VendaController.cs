using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Services;

namespace PresenteDeDeus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _service;

        public VendaController(IVendaService service)
        {
            _service = service;
        }

        // GET /api/venda
        // User.Claims vem do JWT automaticamente após autenticação
        // 'User' aqui = HttpContext.User (o usuário logado atual)
        [HttpGet]
        public async Task<IActionResult> Listar()
            => Ok(await _service.ListarVendasAsync(User));

        // GET /api/venda/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            try
            {
                var venda = await _service.BuscarPorIdAsync(id, User);
                return venda == null ? NotFound() : Ok(venda);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); // HTTP 403 se Operador tentar ver venda de outro
            }
        }

        // POST /api/venda
        // Ambos os perfis podem registrar vendas
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] NovaVendaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var venda = await _service.RegistrarVendaAsync(dto, User);
                return CreatedAtAction(nameof(Buscar), new { id = venda.Id }, venda);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}
