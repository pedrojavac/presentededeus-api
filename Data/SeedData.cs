using PresenteDeDeus.API.Models;

namespace PresenteDeDeus.API.Data
{
    public static class SeedData
    {
        public static void InicializarBanco(AppDbContext context)
        {
            if (context.Usuarios.Any()) return;

            context.Usuarios.AddRange(
                new Usuario
                {
                    Login = "emerson",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("796510"),
                    NomeCompleto = "Emerson (Administrador)",
                    Perfil = PerfilUsuario.Admin
                },
                new Usuario
                {
                    Login = "santinha",
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword("vendas123"),
                    NomeCompleto = "Operador de Vendas",
                    Perfil = PerfilUsuario.Operador
                }
            );

            context.SaveChanges();
        }
    }
}
