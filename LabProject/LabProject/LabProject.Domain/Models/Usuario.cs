using System;

namespace LabProject.Domain.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
        {
            throw new InvalidOperationException("El nombre del usuario es requerido.");
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        {
            throw new InvalidOperationException("El correo electrónico del usuario es inválido.");
        }

        if (string.IsNullOrWhiteSpace(Rol))
        {
            throw new InvalidOperationException("El rol del usuario es requerido.");
        }
    }
}
