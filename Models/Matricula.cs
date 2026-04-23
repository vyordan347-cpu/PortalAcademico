using System.ComponentModel.DataAnnotations;
using PortalAcademico.Data.Enums;

namespace PortalAcademico.Models
{
    public class Matricula
    {
        public int Id { get; set; }

        [Required]
        public int CursoId { get; set; }

        public Curso? Curso { get; set; }

        [Required]
        public string UsuarioId { get; set; } = string.Empty;

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Required]
        public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
    }
}