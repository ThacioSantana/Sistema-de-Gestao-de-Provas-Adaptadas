namespace SistemaDeChamadosNAA.DTOs
{
    public class ChamadoParaProvaDto
    {
        public int ChamadoId { get; set; }
        public string AlunoNome { get; set; }
        public string AlunoMatricula { get; set; }
        public string CursoNome { get; set; }
        public string Descricao { get; set; }
        public DateTime DataAbertura  { get; set; }
        public string Status { get; set; }
    }
}