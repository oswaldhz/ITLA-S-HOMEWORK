using System;
using System.Collections.Generic;
using System.Linq;

namespace LabProject.Domain.Models;

public class Reserva
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int EquipoId { get; set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public ICollection<ReservaSoftware> ReservaSoftware { get; set; } = new List<ReservaSoftware>();

    public void Programar(DateTime fechaInicio, DateTime fechaFin)
    {
        ValidarRango(fechaInicio, fechaFin);
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
    }

    public bool SeSuperponeCon(DateTime inicio, DateTime fin)
    {
        ValidarRango(inicio, fin);
        return inicio < FechaFin && fin > FechaInicio;
    }

    public void ValidarDisponibilidad(IEnumerable<Reserva> reservasExistentes)
    {
        if (reservasExistentes == null)
        {
            throw new ArgumentNullException(nameof(reservasExistentes));
        }

        if (reservasExistentes.Any(r => r.EquipoId == EquipoId && r.SeSuperponeCon(FechaInicio, FechaFin)))
        {
            throw new InvalidOperationException("El equipo ya está reservado en el rango de tiempo solicitado.");
        }
    }

    public void AgregarSoftware(Software software)
    {
        if (software == null)
        {
            throw new ArgumentNullException(nameof(software));
        }

        if (ReservaSoftware.Any(rs => rs.SoftwareId == software.Id))
        {
            return;
        }

        ReservaSoftware.Add(new ReservaSoftware
        {
            ReservaId = Id,
            Reserva = this,
            SoftwareId = software.Id,
            Software = software
        });
    }

    private static void ValidarRango(DateTime inicio, DateTime fin)
    {
        if (fin <= inicio)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }
    }
}
