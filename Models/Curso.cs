using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class Curso
    {
        [Column("id")]
        public int Id { get; set; }
        
        [Column("nome")]
        public string Nome { get; set; }

        [Column("coordenador_id")]
        public int CoordenadorId { get; set; }
        
        [ForeignKey("CoordenadorId")]
        public Coordenador Coordenador { get; set; }

        public List<Aluno> Alunos { get; set; }
        public List<Chamado> Chamados { get; set; }
    }

}