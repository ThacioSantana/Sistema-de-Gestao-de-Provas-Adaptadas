using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SistemaDeChamadosNAA.Models;

namespace SistemaDeChamadosNAA.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Coordenador> Coordenadores { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<UsuarioNpa> UsuariosNPA { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<ChamadoLog> ChamadoLogs { get; set; }
        public DbSet<ProvaAdaptada> ProvasAdaptadas { get; set; }
        public DbSet<HistoricoStatus> HistoricoStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configuração para converter todos DateTime para UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(
                            new ValueConverter<DateTime, DateTime>(
                                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                    }
                }

                // Mapear nomes das tabelas
                modelBuilder.Entity<Coordenador>().ToTable("coordenadores");
                modelBuilder.Entity<UsuarioNpa>().ToTable("usuarios_npa");
                modelBuilder.Entity<Curso>().ToTable("cursos");
                modelBuilder.Entity<Aluno>().ToTable("alunos");
                modelBuilder.Entity<Chamado>().ToTable("chamados");
                modelBuilder.Entity<ChamadoLog>().ToTable("chamado_logs");
                modelBuilder.Entity<ProvaAdaptada>().ToTable("provas_adaptadas");
                modelBuilder.Entity<HistoricoStatus>().ToTable("historico_status");

                // Configurar nomes de colunas para Coordenador
                modelBuilder.Entity<Coordenador>(entity =>
                {
                    entity.Property(c => c.Id).HasColumnName("id");
                    entity.Property(c => c.Matricula).HasColumnName("matricula");
                    entity.Property(c => c.Senha).HasColumnName("senha");
                    entity.Property(c => c.Nome).HasColumnName("nome");
                    entity.Property(c => c.Email).HasColumnName("email");
                    entity.Property(c => c.Perfil).HasColumnName("perfil");
                });

                // Configurar nomes de colunas para UsuarioNpa
                modelBuilder.Entity<UsuarioNpa>(entity =>
                {
                    entity.Property(c => c.Id).HasColumnName("id");
                    entity.Property(n => n.Matricula).HasColumnName("matricula");
                    entity.Property(n => n.Senha).HasColumnName("senha");
                    entity.Property(n => n.Nome).HasColumnName("nome");
                    entity.Property(n => n.Email).HasColumnName("email");
                    entity.Property(n => n.Perfil).HasColumnName("perfil");
                });

                // Relacionamentos principais
                modelBuilder.Entity<Chamado>(entity =>
{
    entity.ToTable("chamados");

    entity.Property(c => c.Id).HasColumnName("id");
    entity.Property(c => c.AlunoId).HasColumnName("aluno_id");
    entity.Property(c => c.CursoId).HasColumnName("curso_id");
    entity.Property(c => c.CoordenadorId).HasColumnName("coordenador_id");
    entity.Property(c => c.NpaId).HasColumnName("npa_id");
    entity.Property(c => c.Descricao).HasColumnName("descricao");
    entity.Property(c => c.DataAbertura).HasColumnName("data_abertura");
    entity.Property(c => c.Status).HasColumnName("status");

    entity.HasOne(c => c.Aluno)
          .WithMany(a => a.Chamados)
          .HasForeignKey(c => c.AlunoId);

    entity.HasOne(c => c.Curso)
          .WithMany(cu => cu.Chamados)
          .HasForeignKey(c => c.CursoId);

    entity.HasOne(c => c.Coordenador)
          .WithMany()
          .HasForeignKey(c => c.CoordenadorId);

    entity.HasOne(c => c.NPA)
          .WithMany()
          .HasForeignKey(c => c.NpaId);

    entity.HasOne(c => c.Prova)
          .WithOne(p => p.Chamado)
          .HasForeignKey<ProvaAdaptada>(p => p.ChamadoId);
});

                modelBuilder.Entity<ChamadoLog>(entity =>
                {
                    entity.ToTable("chamado_logs");
    
                    // Mapeamento completo das propriedades
                    entity.Property(l => l.Id).HasColumnName("id");
                    entity.Property(l => l.ChamadoId).HasColumnName("chamado_id");
                    entity.Property(l => l.acao).HasColumnName("acao");  
                    entity.Property(l => l.Usuario).HasColumnName("usuario");
                    entity.Property(l => l.DataHora).HasColumnName("datahora");

                    // Configuração do relacionamento
                    entity.HasOne(l => l.Chamado)
                        .WithMany()
                        .HasForeignKey(l => l.ChamadoId)
                        .IsRequired();
                });

                modelBuilder.Entity<HistoricoStatus>(entity =>
                {
                    entity.Property(h => h.Id).HasColumnName("id");
                    entity.Property(h => h.ChamadoId).HasColumnName("chamado_id");
                    entity.Property(h => h.StatusAnterior).HasColumnName("status_anterior");
                    entity.Property(h => h.StatusAtual).HasColumnName("status_atual");
                    entity.Property(h => h.DataAlteracao).HasColumnName("data_alteracao");

                    entity.HasOne(h => h.Chamado)
                        .WithMany(c => c.HistoricoStatus)
                        .HasForeignKey(h => h.ChamadoId)
                        .IsRequired();
                });

                
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}