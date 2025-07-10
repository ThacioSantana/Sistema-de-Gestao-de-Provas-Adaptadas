using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class Aluno
    {
        [Column("id")] 
        public int Id { get; set; }
        
        [Column("nome")]
        public string Nome { get; set; }
        
        [Column("email")]
        public string Email { get; set; }
        
        [Column("matricula")]
        public string Matricula { get; set; }
        
        [Column("tipo_deficiencia")]
        public string TipoDeficiencia { get; set; }

        [Column("curso_id")]
        public int CursoId { get; set; }
        
        public Curso Curso { get; set; }
        public List<Chamado> Chamados { get; set; } // Relacionamento com chamados
    }
}