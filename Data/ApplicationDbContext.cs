using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PortalAcademico.Models;

namespace PortalAcademico.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Curso>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Codigo)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(c => c.Codigo)
                    .IsUnique();

                entity.Property(c => c.Nombre)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Activo)
                    .HasDefaultValue(true);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Curso_Creditos_MayorQueCero", "\"Creditos\" > 0");
                    t.HasCheckConstraint("CK_Curso_CupoMaximo_MayorQueCero", "\"CupoMaximo\" > 0");
                    t.HasCheckConstraint("CK_Curso_HorarioInicio_MenorHorarioFin", "\"HorarioInicio\" < \"HorarioFin\"");
                });
            });

            builder.Entity<Matricula>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.UsuarioId)
                    .IsRequired();

                entity.Property(m => m.FechaRegistro)
                    .IsRequired();

                entity.Property(m => m.Estado)
                    .IsRequired();

                entity.HasOne(m => m.Curso)
                    .WithMany(c => c.Matriculas)
                    .HasForeignKey(m => m.CursoId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(m => new { m.CursoId, m.UsuarioId })
                    .IsUnique();
            });
        }
    }
}