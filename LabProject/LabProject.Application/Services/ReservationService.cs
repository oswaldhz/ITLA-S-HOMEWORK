using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;

namespace LabProject.Application.Services;

public class ReservationService : IReservationService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEquipoRepository _equipoRepository;
    private readonly ISoftwareRepository _softwareRepository;

    public ReservationService(
        IReservaRepository reservaRepository,
        IUsuarioRepository usuarioRepository,
        IEquipoRepository equipoRepository,
        ISoftwareRepository softwareRepository)
    {
        _reservaRepository = reservaRepository;
        _usuarioRepository = usuarioRepository;
        _equipoRepository = equipoRepository;
        _softwareRepository = softwareRepository;
    }

    public async Task<ReservationValidationResult> ValidateAsync(CreateReservaRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.FechaFin <= request.FechaInicio)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("El usuario no existe.");

        var equipo = await _equipoRepository.GetByIdAsync(request.EquipoId, cancellationToken)
            ?? throw new InvalidOperationException("El equipo no existe.");

        if (equipo.Estado.Equals("Mantenimiento", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El equipo está en mantenimiento y no puede reservarse.");
        }

        var reservasEnRango = await _reservaRepository.GetReservasDeEquipoEnRangoAsync(
            equipo.Id,
            request.FechaInicio,
            request.FechaFin,
            cancellationToken);

        if (reservasEnRango.Any(r => request.FechaInicio < r.FechaFin && request.FechaFin > r.FechaInicio))
        {
            throw new InvalidOperationException("El equipo ya está reservado en el rango de tiempo solicitado.");
        }

        var softwares = await _softwareRepository.GetByIdsAsync(request.SoftwareIds, cancellationToken);
        var missingSoftware = request.SoftwareIds.Except(softwares.Select(s => s.Id)).ToList();
        if (missingSoftware.Any())
        {
            throw new InvalidOperationException("Uno o más programas solicitados no existen o no están instalados en el equipo.");
        }

        return new ReservationValidationResult(usuario, equipo, softwares, reservasEnRango);
    }

    public async Task<ReservationValidationResult> ValidateForUpdateAsync(int reservaId, CreateReservaRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.FechaFin <= request.FechaInicio)
        {
            throw new InvalidOperationException("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("El usuario no existe.");

        var equipo = await _equipoRepository.GetByIdAsync(request.EquipoId, cancellationToken)
            ?? throw new InvalidOperationException("El equipo no existe.");

        if (equipo.Estado.Equals("Mantenimiento", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("El equipo está en mantenimiento y no puede reservarse.");
        }

        var reservasEnRango = await _reservaRepository.GetReservasDeEquipoEnRangoAsync(
            equipo.Id,
            request.FechaInicio,
            request.FechaFin,
            cancellationToken);

        // Excluir la misma reserva al evaluar solapamientos.
        reservasEnRango = reservasEnRango.Where(r => r.Id != reservaId).ToList();

        if (reservasEnRango.Any(r => request.FechaInicio < r.FechaFin && request.FechaFin > r.FechaInicio))
        {
            throw new InvalidOperationException("El equipo ya está reservado en el rango de tiempo solicitado.");
        }

        var softwares = await _softwareRepository.GetByIdsAsync(request.SoftwareIds, cancellationToken);
        var missingSoftware = request.SoftwareIds.Except(softwares.Select(s => s.Id)).ToList();
        if (missingSoftware.Any())
        {
            throw new InvalidOperationException("Uno o más programas solicitados no existen o no están instalados en el equipo.");
        }

        return new ReservationValidationResult(usuario, equipo, softwares, reservasEnRango);
    }
}
