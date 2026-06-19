namespace PresenteDeDeus.API.DTOs
{
    // Objeto recebido do front-end ao fazer login
    public class LoginRequestDto
    {
        public string Login { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    // Objeto enviado de volta ao front-end após login com sucesso
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty; // Token JWT
        public string NomeCompleto { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty; // 'Admin' ou 'Operador'
        public DateTime Expiracao { get; set; }                 // Quando o token expira
    }
}
