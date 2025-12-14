using System;

namespace LabProject.Domain.Models;

public class Equipo
{
    public int Id { get; set; }
    public string Identificador { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;

    public void CambiarEstado(string nuevoEstado)
    {
        if (string.IsNullOrWhiteSpace(nuevoEstado))
        {
            throw new InvalidOperationException("El estado del equipo es requerido.");
        }

        Estado = nuevoEstado;
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Identificador))
        {
            throw new InvalidOperationException("El identificador del equipo es requerido.");
        }

        if (string.IsNullOrWhiteSpace(Ubicacion))
        {
            throw new InvalidOperationException("La ubicación del equipo es requerida.");
        }
    }
}
