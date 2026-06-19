using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Services;

namespace PresenteDeDeus.API.Controllers
{
    [ApiController]               // Habilita validação automática de Model
    [Route("api/[controller]")]    // Rota base: /api/auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST /api/auth/login
        // [AllowAnonymous] = sem necessidade de token (rota pública)
        // Equivalente Java: @PostMapping("/login") sem @PreAuthorize
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            // [FromBody] = @RequestBody no Java
            var resultado = await _authService.LoginAsync(dto);

            if (resultado == null)
                // HTTP 401 — login inválido
                return Unauthorized(new { mensagem = "Login ou senha incorretos." });

            // HTTP 200 com o token JWT no corpo
            return Ok(resultado);
        }
    }
}
