using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaDeChamadosNAA.Models
{
    public class ProvaAdaptada
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("chamado_id")]
        public int ChamadoId { get; set; }
        public Chamado Chamado { get; set; }

        [Column("arquivo_prova")] // <-- AQUI ESTÁ O ERRO
        public string ArquivoProva { get; set; }

        [Column("data_envio")]
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

        [Column("status")]
        public string Status { get; set; }

        [Column("observacoes")]
        public string Observacoes { get; set; }
    }
}