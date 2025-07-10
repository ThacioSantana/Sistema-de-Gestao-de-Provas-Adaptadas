using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistemaDeChamadosNAA.Data;
using SistemaDeChamadosNAA.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistemaDeChamadosNAA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var matricula = request.Matricula.Trim();
            var senhaDigitada = request.Senha;

            var coordenador = _context.Coordenadores
                .AsNoTracking()
                .FirstOrDefault(c => c.Matricula == matricula);

            var usuarioNpa = _context.UsuariosNPA
                .AsNoTracking()
                .FirstOrDefault(u => u.Matricula == matricula);

            Console.WriteLine("🔍 Iniciando validação de login...");
            Console.WriteLine($"📩 Matrícula recebida: {request.Matricula}");
            Console.WriteLine($"🔑 Senha recebida: {request.Senha}");

            if (coordenador != null)
            {
                Console.WriteLine($"👤 Coordenador encontrado: {coordenador.Nome}");
                Console.WriteLine($"🔑 Senha no banco (coordenador): {coordenador.Senha}");
                Console.WriteLine($"✅ Senha válida? {senhaDigitada == coordenador.Senha}");
            }
            else if (usuarioNpa != null)
            {
                Console.WriteLine($"👤 Usuário NAA encontrado: {usuarioNpa.Nome}");
                Console.WriteLine($"🔑 Senha no banco (usuarioNpa): {usuarioNpa.Senha}");
                Console.WriteLine($"✅ Senha válida? {senhaDigitada == usuarioNpa.Senha}");
            }
            else
            {
                Console.WriteLine("❌ Usuário não encontrado.");
            }
            
            if (coordenador == null && usuarioNpa == null)
                return Unauthorized(new { mensagem = "Matrícula não encontrada." });

            var senhaBanco = coordenador?.Senha ?? usuarioNpa?.Senha ?? "";

            // Substituição da verificação BCrypt por comparação simples
            if (senhaDigitada != senhaBanco)
                return Unauthorized(new { mensagem = "Senha incorreta." });

            var perfil = coordenador != null ? "Coordenador" : "NAA";
            var id = coordenador?.Id ?? usuarioNpa!.Id;
            var nome = coordenador?.Nome ?? usuarioNpa!.Nome;

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, perfil),
                new Claim("Matricula", matricula)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDto
            {
                Mensagem = "Login realizado com sucesso.",
                Token = tokenString,
                Perfil = perfil,
                Id = id,
                Matricula = matricula,
                Nome = nome
            };

            return Ok(response);
        }
    }
}