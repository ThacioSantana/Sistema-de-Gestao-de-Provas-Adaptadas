using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class Coordenador
    {
        [Column("id")]
        public int Id { get; set; } 
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        [Column("email")]
        public string Email { get; set; } = string.Empty;
        [Column("matricula")]
        public string Matricula { get; set; } = string.Empty;
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;
        [Column("perfil")]
        public string Perfil { get; set; } = string.Empty;
    }
}