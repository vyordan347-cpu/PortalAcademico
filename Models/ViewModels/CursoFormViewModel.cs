using System.ComponentModel.DataAnnotations;

namespace PortalAcademico.Models.ViewModels
{
    public class CursoFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Codigo { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Los créditos deben ser mayores a 0.")]
        public int Creditos { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cupo máximo debe ser mayor a 0.")]
        public int CupoMaximo { get; set; }

        [Required]
        public TimeOnly HorarioInicio { get; set; }

        [Required]
        public TimeOnly HorarioFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}