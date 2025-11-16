namespace LaboratorioAPI.DTOs
{
    public class EquipoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NumeroSerie { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Especificaciones { get; set; } = string.Empty;
        public string Ubicacion { get; set; } = string.Empty;
        public DateTime FechaAdquisicion { get; set; }
        public DateTime? UltimoMantenimiento { get; set; }
        public bool Activo { get; set; }
    }
}