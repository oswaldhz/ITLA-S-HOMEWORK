using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEquipoRepository _equipoRepository;
    private readonly ISoftwareRepository _softwareRepository;
    private readonly IReservationService _reservationService;

    public ReservaService(
        IReservaRepository reservaRepository,
        IUsuarioRepository usuarioRepository,
        IEquipoRepository equipoRepository,
        ISoftwareRepository softwareRepository,
        IReservationService reservationService)
    {
        _reservaRepository = reservaRepository;
        _usuarioRepository = usuarioRepository;
        _equipoRepository = equipoRepository;
        _softwareRepository = softwareRepository;
        _reservationService = reservationService;
    }

    public async Task<bool> CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _reservaRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ReservaDto> CrearAsync(CreateReservaRequest request, ReservationValidationResult? validationResult = null, CancellationToken cancellationToken = default)
    {
        validationResult ??= await _reservationService.ValidateAsync(request, cancellationToken);

        var reserva = new Reserva
        {
            UsuarioId = validationResult.Usuario.Id,
            EquipoId = validationResult.Equipo.Id
        };
        reserva.Programar(request.FechaInicio, request.FechaFin);
        reserva.ValidarDisponibilidad(validationResult.ReservasEnRango);

        foreach (var software in validationResult.Softwares)
        {
            reserva.AgregarSoftware(software);
        }

        var creada = await _reservaRepository.AddAsync(reserva, cancellationToken);
        return Map(creada, validationResult.Usuario, validationResult.Equipo);
    }

    public async Task<IReadOnlyList<ReservaDto>> ObtenerPorFiltrosAsync(DateTime? fecha, int? equipoId, int? usuarioId, CancellationToken cancellationToken = default)
    {
        var reservas = await _reservaRepository.GetByFiltersAsync(fecha, equipoId, usuarioId, cancellationToken);
        return await MapReservas(reservas, cancellationToken);
    }

    public async Task<ReservaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var reserva = await _reservaRepository.GetByIdAsync(id, cancellationToken);
        if (reserva == null)
        {
            return null;
        }

        var usuario = await _usuarioRepository.GetByIdAsync(reserva.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("El usuario asociado a la reserva no existe.");
        var equipo = await _equipoRepository.GetByIdAsync(reserva.EquipoId, cancellationToken)
            ?? throw new InvalidOperationException("El equipo asociado a la reserva no existe.");

        return Map(reserva, usuario, equipo);
    }

    private async Task<IReadOnlyList<ReservaDto>> MapReservas(IEnumerable<Reserva> reservas, CancellationToken cancellationToken)
    {
        var usuarioIds = reservas.Select(r => r.UsuarioId).Distinct().ToList();
        var equipoIds = reservas.Select(r => r.EquipoId).Distinct().ToList();

        var usuarios = await _usuarioRepository.GetAllAsync(cancellationToken);
        var equipos = await _equipoRepository.GetAllAsync(cancellationToken);

        var usuariosDict = usuarios.Where(u => usuarioIds.Contains(u.Id)).ToDictionary(u => u.Id);
        var equiposDict = equipos.Where(e => equipoIds.Contains(e.Id)).ToDictionary(e => e.Id);

        return reservas
            .Select(reserva =>
            {
                usuariosDict.TryGetValue(reserva.UsuarioId, out var usuario);
                equiposDict.TryGetValue(reserva.EquipoId, out var equipo);
                return Map(reserva, usuario ?? new Usuario(), equipo ?? new Equipo());
            })
            .ToList();
    }

    private static ReservaDto Map(Reserva reserva, Usuario usuario, Equipo equipo)
    {
        var softwareIds = reserva.ReservaSoftware.Select(rs => rs.SoftwareId).ToList();
        return new ReservaDto(
            reserva.Id,
            reserva.UsuarioId,
            usuario.Nombre,
            reserva.EquipoId,
            equipo.Identificador,
            reserva.FechaInicio,
            reserva.FechaFin,
            softwareIds);
    }
}
