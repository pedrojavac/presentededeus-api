using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PresenteDeDeus.API.Data;
using PresenteDeDeus.API.DTOs;
using PresenteDeDeus.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PresenteDeDeus.API.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
        string HashSenha(string senha);
    }

    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Login.ToLower() == dto.Login.ToLower() && u.Ativo);

            if (usuario == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            {
                return null;
            }

            var expiracao = DateTime.Now.AddHours(
                double.Parse(_config["Jwt:ExpiracaoHoras"]!));
            var token = GerarToken(usuario, expiracao);

            return new LoginResponseDto
            {
                Token = token,
                NomeCompleto = usuario.NomeCompleto,
                Perfil = usuario.Perfil.ToString(),
                Expiracao = expiracao
            };
        }

        public string HashSenha(string senha)
            => BCrypt.Net.BCrypt.HashPassword(senha);

        private string GerarToken(Usuario usuario, DateTime expiracao)
        {
            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Login),
                new Claim(ClaimTypes.Role, usuario.Perfil.ToString()),
                new Claim("NomeCompleto", usuario.NomeCompleto),
                new Claim("UsuarioId", usuario.Id.ToString())
            };

            var creds = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

            var tokenJwt = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: expiracao,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenJwt);
        }
    }
}
