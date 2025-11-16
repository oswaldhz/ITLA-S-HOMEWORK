using System.ComponentModel.DataAnnotations;

namespace LaboratorioAPI.Models
{
    public class Equipo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del equipo es requerido")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de serie es requerido")]
        public string NumeroSerie { get; set; } = string.Empty;

        [Required(ErrorMessage = "La marca es requerida")]
        public string Marca { get; set; } = string.Empty;

        [Required(ErrorMessage = "El modelo es requerido")]
        public string Modelo { get; set; } = string.Empty;

        public string Estado { get; set; } = "Disponible";

        public string Especificaciones { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ubicación es requerida")]
        public string Ubicacion { get; set; } = string.Empty;

        public DateTime FechaAdquisicion { get; set; } = DateTime.UtcNow;
        public DateTime? UltimoMantenimiento { get; set; }
        public bool Activo { get; set; } = true;
    }
}