using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamadosNAA.Data;

namespace SistemaDeChamadosNAA.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public EmailService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task EnviarEmail(string destino, string assunto, string mensagem)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Smtp:User"]));
            email.To.Add(MailboxAddress.Parse(destino));
            email.Subject = assunto;
            email.Body = new TextPart("plain") { Text = mensagem };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _configuration["Smtp:Host"],
                int.Parse(_configuration["Smtp:Port"]),
                bool.Parse(_configuration["Smtp:UseSsl"])
            );
            await smtp.AuthenticateAsync(
                _configuration["Smtp:User"],
                _configuration["Smtp:Password"]
            );
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task EnviarEmailParaAluno(int alunoId, string assunto, string mensagem)
        {
            var aluno = await _context.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId);
            if (aluno != null)
                await EnviarEmail(aluno.Email, assunto, mensagem);
        }

        public async Task EnviarEmailParaCoordenador(int coordenadorId, string assunto, string mensagem)
        {
            var coord = await _context.Coordenadores.FirstOrDefaultAsync(c => c.Id == coordenadorId);
            if (coord != null)
                await EnviarEmail(coord.Email, assunto, mensagem);
        }

        public async Task EnviarEmailParaNAA(int npaId, string assunto, string mensagem)
        {
            var npa = await _context.UsuariosNPA.FirstOrDefaultAsync(u => u.Id == npaId);
            if (npa != null)
                await EnviarEmail(npa.Email, assunto, mensagem);
        }
    }
}
