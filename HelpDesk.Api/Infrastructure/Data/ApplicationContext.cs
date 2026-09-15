using HelpDesk.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Tecnico> Tecnicos { get; set; }
        public DbSet<Chamado> Chamados { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.Property(x => x.IdUsuario).ValueGeneratedNever();
                entity.Property(x => x.Nome).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Email).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Departamento).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Ativo).HasConversion<int>().HasColumnType("NUMBER(1)");
            });

            modelBuilder.Entity<Tecnico>(entity =>
            {
                entity.Property(x => x.IdTecnico).ValueGeneratedNever();
                entity.Property(x => x.Nome).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Email).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Especialidade).HasColumnType("VARCHAR2(50)");
                entity.Property(x => x.Ativo).HasConversion<int>().HasColumnType("NUMBER(1)");
            });

            modelBuilder.Entity<Chamado>(entity =>
            {
                entity.Property(x => x.IdChamado).ValueGeneratedNever();
                entity.Property(x => x.Titulo).HasColumnType("VARCHAR2(150)");
                entity.Property(x => x.Descricao).HasColumnType("VARCHAR2(1000)");
                entity.Property(x => x.Prioridade).HasColumnType("VARCHAR2(20)");
                entity.Property(x => x.Status).HasColumnType("VARCHAR2(30)");
                entity.Property(x => x.DataAbertura).HasColumnType("TIMESTAMP");
                entity.Property(x => x.DataAtualizacao).HasColumnType("TIMESTAMP");
                entity.Property(x => x.DataFechamento).HasColumnType("TIMESTAMP");

                entity.HasOne(x => x.Usuario)
                    .WithMany(x => x.Chamados)
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_CHAMADO_USUARIO");

                entity.HasOne(x => x.Tecnico)
                    .WithMany(x => x.Chamados)
                    .HasForeignKey(x => x.IdTecnico)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_CHAMADO_TECNICO");
            });

            modelBuilder.Entity<Comentario>(entity =>
            {
                entity.Property(x => x.IdComentario).ValueGeneratedNever();
                entity.Property(x => x.Autor).HasColumnType("VARCHAR2(100)");
                entity.Property(x => x.Texto).HasColumnType("VARCHAR2(1000)");
                entity.Property(x => x.DataCriacao).HasColumnType("TIMESTAMP");

                entity.HasOne(x => x.Chamado)
                    .WithMany(x => x.Comentarios)
                    .HasForeignKey(x => x.IdChamado)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_COMENTARIO_CHAMADO");
            });
        }
    }
}
