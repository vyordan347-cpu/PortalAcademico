using PortalAcademico.Models;

namespace PortalAcademico.Models.ViewModels
{
    public class CatalogoCursosViewModel
    {
        public string? Nombre { get; set; }

        public int? CreditosMin { get; set; }

        public int? CreditosMax { get; set; }

        public TimeOnly? HorarioInicio { get; set; }

        public TimeOnly? HorarioFin { get; set; }

        public List<Curso> Cursos { get; set; } = new();
    }
}