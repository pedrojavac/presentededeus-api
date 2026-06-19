using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Services;

namespace PresenteDeDeus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _service;

        public ProdutoController(IProdutoService service)
        {
            _service = service;
        }

        // GET /api/produto
        // Qualquer usuário logado pode listar os produtos
        [HttpGet]
        public async Task<IActionResult> Listar()
            => Ok(await _service.ListarAsync());

        // GET /api/produto/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var produto = await _service.BuscarPorIdAsync(id);
            return produto == null ? NotFound() : Ok(produto);
        }

      
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Criar([FromBody] ProdutoRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // HTTP 400 se validação falhar

            var criado = await _service.CriarAsync(dto);
            // HTTP 201 Created com o Location header apontando para o novo recurso
            return CreatedAtAction(nameof(Buscar), new { id = criado.Id }, criado);
        }

        // PUT /api/produto/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Atualizar(
            int id, [FromBody] ProdutoRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var atualizado = await _service.AtualizarAsync(id, dto);
            return atualizado == null ? NotFound() : Ok(atualizado);
        }

        // DELETE /api/produto/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Remover(int id)
        {
            var removido = await _service.RemoverAsync(id);
            return removido ? NoContent() : NotFound();
            // NoContent = HTTP 204 — sucesso sem corpo na resposta
        }
    }
}
