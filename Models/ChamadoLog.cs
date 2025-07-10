using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaDeChamadosNAA.Models;

[Table("chamado_logs")]
public class ChamadoLog
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("chamado_id")]
    public int ChamadoId { get; set; }
    
    [Column("acao")]
    public string acao { get; set; } = string.Empty;
    
    [Column("usuario")]
    public string Usuario { get; set; } = string.Empty;
    
    [Column("datahora")]
    public DateTime DataHora { get; set; }
    
    [ForeignKey("ChamadoId")]
    public Chamado Chamado { get; set; }
}
