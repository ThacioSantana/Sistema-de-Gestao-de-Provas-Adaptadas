namespace SistemaDeChamadosNAA.Models
{
    public class UsuarioNpa
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Perfil { get; init; }
    }
}