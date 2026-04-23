using PortalAcademico.Models;

namespace PortalAcademico.Models.ViewModels
{
    public class CursoMatriculasViewModel
    {
        public Curso Curso { get; set; } = null!;
        public List<Matricula> Matriculas { get; set; } = new();
    }
}