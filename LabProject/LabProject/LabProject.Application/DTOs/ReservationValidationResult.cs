using LabProject.Domain.Models;

namespace LabProject.Application.DTOs;

public record ReservationValidationResult(
    Usuario Usuario,
    Equipo Equipo,
    IReadOnlyList<Software> Softwares,
    IReadOnlyList<Reserva> ReservasEnRango);
