using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaDeChamadosNAA.Data;
using SistemaDeChamadosNAA.Models;
using SistemaDeChamadosNAA.Services;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using SistemaDeChamadosNAA.DTOs;

namespace SistemaDeChamadosNAA.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;

        public ChamadoController(AppDbContext context, IUsuarioService usuarioService, IEmailService emailService)
        {
            _context = context;
            _usuarioService = usuarioService;
            _emailService = emailService;
        }
        
        [Authorize(Roles = "NAA")]
        [HttpPost("criar")]
        public IActionResult CriarChamado([FromBody] ChamadoCriacaoDto dto)
        {
            var perfil = _usuarioService.GetPerfilUsuario(HttpContext);
            if (perfil != "NAA")
                return Unauthorized("Apenas o NAA pode criar chamados.");

            if (string.IsNullOrWhiteSpace(dto.AlunoMatricula) || dto.CursoId == 0 || string.IsNullOrWhiteSpace(dto.Descricao))
                return BadRequest("Dados inválidos.");

            // Carrega o aluno com o curso incluso
            var aluno = _context.Alunos
                .Include(a => a.Curso)
                .FirstOrDefault(a => a.Matricula == dto.AlunoMatricula.Trim());

            if (aluno == null || aluno.Curso == null)
                return NotFound("Aluno ou curso não encontrado.");

            // Busca o coordenador responsável pelo curso
            var coordenador = _context.Coordenadores.FirstOrDefault(c => c.Id == aluno.Curso.CoordenadorId);

            // Recupera o ID do usuário logado (NAA)
            var usuarioId = _usuarioService.GetUsuarioId(HttpContext);

            // Cria o chamado
            var chamado = new Chamado
            {
                AlunoId = aluno.Id,
                CursoId = dto.CursoId,
                CoordenadorId = coordenador?.Id ?? 0,
                NpaId = usuarioId,
                Descricao = dto.Descricao,
                Status = "Criado",
                DataAbertura = DateTime.UtcNow
            };


            _context.Chamados.Add(chamado);
            _context.SaveChanges();

            _context.ChamadoLogs.Add(new ChamadoLog
            {
                ChamadoId = chamado.Id,
                acao = $"Chamado criado para o aluno {dto.AlunoMatricula}",
                Usuario = perfil,
                DataHora = DateTime.UtcNow
            });
            _context.SaveChanges();

            if (coordenador != null)
            {
                _emailService.EnviarEmail(coordenador.Email, "Novo Chamado Criado", $"Foi criado um novo chamado para o aluno {aluno.Nome} ({aluno.Matricula}).");
            }

            return Ok(new
            {
                mensagem = "Chamado criado com sucesso!",
                chamado.Id,
                AlunoMatricula = dto.AlunoMatricula
            });
        }

        // 2. visualizar as solicitações de prova (feito pelo Coordenador)
        [Authorize(Roles = "Coordenador")]
        [HttpGet("pendentes-prova")]
        public IActionResult GetChamadosPendentesProva()
        {
            // Recupera o perfil e o ID do coordenador logado a partir do token
            var perfil = _usuarioService.GetPerfilUsuario(HttpContext);
            if (perfil != "Coordenador")
                return Forbid("Apenas coordenadores podem visualizar esta lista.");

            int coordenadorId = _usuarioService.GetUsuarioId(HttpContext);

            var chamados = _context.Chamados
                .Where(c => c.CoordenadorId == coordenadorId)
                .Include(c => c.Aluno)
                .Include(c => c.Curso)
                .Select(c => new ChamadoParaProvaDto
                {
                    ChamadoId = c.Id,
                    AlunoNome = c.Aluno.Nome,
                    AlunoMatricula = c.Aluno.Matricula,
                    CursoNome = c.Curso.Nome,
                    Descricao = c.Descricao,
                    DataAbertura = c.DataAbertura,
                    Status = c.Status
                })
                .ToList();

            return Ok(chamados);
        }

        [HttpPost("enviar-prova/{id}")]
        public async Task<IActionResult> EnviarProva(int id, IFormFile arquivo)
        {
            var chamado = _context.Chamados.Include(c => c.Aluno).FirstOrDefault(c => c.Id == id);
            if (chamado == null) return NotFound("Chamado não encontrado.");

            var perfil = _usuarioService.GetPerfilUsuario(HttpContext);
            if (perfil != "Coordenador") return Forbid("Apenas o Coordenador pode enviar provas.");

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var extensoesPermitidas = new[] { ".pdf", ".doc", ".docx" };
            var extensao = Path.GetExtension(arquivo.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extensao))
                return BadRequest("Tipo de arquivo não permitido. Envie um PDF ou Word (.pdf, .doc, .docx).");

            var pasta = Path.Combine("Uploads", "Provas");
            Directory.CreateDirectory(pasta);

            var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
            var caminho = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var prova = new ProvaAdaptada
            {
                ArquivoProva = caminho,
                Status = "Enviado",
                Observacoes = string.Empty,
                ChamadoId = chamado.Id
            };

            chamado.Prova = prova;
            chamado.Status = "Prova enviada";

            _context.Chamados.Update(chamado);
            _context.ProvasAdaptadas.Add(prova);
            _context.ChamadoLogs.Add(new ChamadoLog
            {
                ChamadoId = chamado.Id,
                acao = "Prova enviada",
                Usuario = perfil,
                DataHora = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            var usuarioNpa = _context.UsuariosNPA.FirstOrDefault(u => u.Id == chamado.NpaId);
            if (usuarioNpa != null)
            {
                _emailService.EnviarEmail(usuarioNpa.Email, "Prova enviada para validação",
                    $"A prova adaptada do aluno {chamado.Aluno.Nome} foi enviada e aguarda validação.");
            }

            return Ok("Prova enviada com sucesso!");
        }

        [HttpPost("validar-prova/{id}")]
        public IActionResult ValidarProva(int id, [FromBody] string resultado)
        {
            var chamado = _context.Chamados
                .Include(c => c.Aluno)
                .FirstOrDefault(c => c.Id == id);

            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            var perfil = _usuarioService.GetPerfilUsuario(HttpContext);
            if (perfil != "NAA")
                return Forbid("Apenas o NAA pode validar provas.");

            if (resultado != "Aprovado" && resultado != "Reprovado" && resultado != "Concluído")
                return BadRequest("Resultado inválido. Use 'Aprovado', 'Reprovado' ou 'Concluído'.");

            chamado.Status = resultado;
            _context.Chamados.Update(chamado);

            _context.ChamadoLogs.Add(new ChamadoLog
            {
                ChamadoId = chamado.Id,
                acao = $"Chamado {resultado.ToLower()} pelo NAA",
                Usuario = perfil,
                DataHora = DateTime.UtcNow
            });

            _context.SaveChanges();

            var coordenador = _context.Coordenadores.FirstOrDefault(c => c.Id == chamado.CoordenadorId);
            if (coordenador != null)
            {
                _emailService.EnviarEmail(coordenador.Email, "Status atualizado do chamado", $"O chamado do aluno {chamado.Aluno.Nome} foi marcado como: {resultado}.");
            }

            _emailService.EnviarEmail(chamado.Aluno.Email, "Status do chamado atualizado", $"Seu chamado foi atualizado para: {resultado}.");

            return Ok(new { mensagem = $"Chamado marcado como {resultado.ToLower()} com sucesso!", chamado.Status });
        }
        
        [HttpGet("todos")]
        [Authorize]
        public async Task<IActionResult> ObterTodosChamados()
        {
            var chamados = await _context.Chamados
                .Include(c => c.Aluno)
                .Include(c => c.Prova)
                .Select(c => new
                {
                    Id = c.Id,
                    MatriculaAluno = c.Aluno.Matricula,
                    NomeAluno = c.Aluno.Nome,
                    Status = c.Status,
                    DataAbertura = c.DataAbertura,
                    ProvaArquivo = c.Prova != null ? Path.GetFileName(c.Prova.ArquivoProva) : null
                })
                .ToListAsync();

            return Ok(chamados);
        }
        
        [HttpGet("download-prova/{id}")]
        [Authorize(Roles = "NAA")]
        public IActionResult BaixarProva(int id)
        {
            var prova = _context.ProvasAdaptadas.FirstOrDefault(p => p.ChamadoId == id);
            if (prova == null || string.IsNullOrEmpty(prova.ArquivoProva) || !System.IO.File.Exists(prova.ArquivoProva))
            {
                return NotFound("Arquivo da prova não encontrado.");
            }

            var bytes = System.IO.File.ReadAllBytes(prova.ArquivoProva);
            var nomeArquivo = Path.GetFileName(prova.ArquivoProva);
            return File(bytes, "application/octet-stream", nomeArquivo);
        }
        
        [Authorize(Roles = "NAA")]
        [HttpGet("aluno/{matricula}")]
        public IActionResult GetAlunoPorMatricula(string matricula)
        {
            var aluno = _context.Alunos
                .Include(a => a.Curso)
                .ThenInclude(c => c.Coordenador)
                .FirstOrDefault(a => a.Matricula == matricula);

            if (aluno == null)
                return NotFound("Aluno não encontrado.");

            return Ok(new
            {
                alunoNome = aluno.Nome,
                alunoEmail = aluno.Email,
                cursoId = aluno.Curso.Id,
                cursoNome = aluno.Curso.Nome,
                coordenadorNome = aluno.Curso.Coordenador?.Nome,
                coordenadorEmail = aluno.Curso.Coordenador?.Email
            });
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "NAA")]
        public IActionResult GetChamadoPorId(int id)
        {
            var chamado = _context.Chamados
                .Include(c => c.Aluno)
                .Include(c => c.Curso)
                .Include(c => c.Prova)
                .FirstOrDefault(c => c.Id == id);

            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            return Ok(new
            {
                id = chamado.Id,
                nomeAluno = chamado.Aluno.Nome,
                matriculaAluno = chamado.Aluno.Matricula,
                emailAluno = chamado.Aluno.Email,
                nomeCoordenador = chamado.CoordenadorId != 0 ? _context.Coordenadores.FirstOrDefault(c => c.Id == chamado.CoordenadorId)?.Nome : "",
                emailCoordenador = chamado.CoordenadorId != 0 ? _context.Coordenadores.FirstOrDefault(c => c.Id == chamado.CoordenadorId)?.Email : "",
                curso = chamado.Curso?.Nome ?? "",
                cursoNome = chamado.Curso?.Nome ?? "",
                descricao = chamado.Descricao,
                status = chamado.Status,
                provaArquivo = chamado.Prova?.ArquivoProva != null
            });
        }
    }
}