namespace SistemaDeChamadosNAA.DTOs
{
    public class LoginResponseDto
    {
        public string Mensagem { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Perfil { get; set; } = string.Empty;
        public int Id { get; set; }
        public string Matricula { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}