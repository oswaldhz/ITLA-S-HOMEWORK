using System;

namespace LabProject.Domain.Models;

public class Software
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Licencia { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            throw new InvalidOperationException("El nombre del software es requerido.");
        }

        if (string.IsNullOrWhiteSpace(Version))
        {
            throw new InvalidOperationException("La versión del software es requerida.");
        }
    }
}
