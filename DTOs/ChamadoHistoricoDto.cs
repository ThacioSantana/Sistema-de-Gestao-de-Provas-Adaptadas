namespace SistemaDeChamadosNAA.DTOs;

public class ChamadoHistoricoDto
{
    public int Id { get; set; }
    public string MatriculaAluno { get; set; }
    public string NomeAluno { get; set; }
    public string Status { get; set; }
    public DateTime DataAbertura { get; set; }
}