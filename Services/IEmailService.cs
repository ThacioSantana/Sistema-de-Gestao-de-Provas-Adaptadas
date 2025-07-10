namespace SistemaDeChamadosNAA.Services
{
    public interface IEmailService
    {
        Task EnviarEmail(string destino, string assunto, string mensagem);
        Task EnviarEmailParaAluno(int alunoId, string assunto, string mensagem);
        Task EnviarEmailParaCoordenador(int coordenadorId, string assunto, string mensagem);
        Task EnviarEmailParaNAA(int npaId, string assunto, string mensagem);
    }
}