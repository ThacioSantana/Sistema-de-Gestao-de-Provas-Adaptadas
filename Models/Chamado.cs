using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class Chamado
    {
        public int Id { get; set; }
        
        [Column("aluno_id")]
        public int AlunoId { get; set; }
        [ForeignKey("AlunoId")]
        public Aluno Aluno { get; set; }
        public int CursoId { get; set; }
        public Curso Curso { get; set; }
        public int CoordenadorId { get; set; }
        public Coordenador Coordenador { get; set; }
        public int? NpaId { get; set; }
        public UsuarioNpa NPA { get; set; }
        public string Descricao { get; set; }
        [Column("data_abertura")]
        public DateTime DataAbertura { get; set; }
        public string Status { get; set; }
        public ProvaAdaptada? Prova { get; set; }
        public IEnumerable<HistoricoStatus>? HistoricoStatus { get; set; }
    }
}