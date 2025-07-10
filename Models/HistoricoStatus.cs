using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class HistoricoStatus
    {
        public int Id { get; set; }
        public int ChamadoId { get; set; }
        public Chamado Chamado { get; set; }

        public string Status { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string Comentario { get; set; }
        public string StatusAnterior { get; set; }
        public string StatusAtual { get; set; }
    }
}