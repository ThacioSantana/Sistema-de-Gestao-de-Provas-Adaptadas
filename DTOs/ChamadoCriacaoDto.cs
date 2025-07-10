namespace SistemaDeChamadosNAA.DTOs
{
    public class ChamadoCriacaoDto
    {
        public string AlunoMatricula { get; set; } = string.Empty; // Alterado de AlunoId
        public int CursoId { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
