using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.DTOs
{
    public class UpdateEquipoDto
    {
        [Required(ErrorMessage = "El ID es requerido")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de serie es requerido")]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es requerida")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es requerido")]
        public string Modelo { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;
        public string Especificaciones { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es requerida")]
        public string Ubicacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de adquisición es requerida")]
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? UltimoMantenimiento { get; set; }
        public bool Activo { get; set; }
    }
}