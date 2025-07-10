namespace SistemaDeChamadosNAA.Models;

public class ValidacaoProvaDto
{
    public int ChamadoId { get; set; }
    public bool Aprovado { get; set; }
    public string Comentario { get; set; }
}