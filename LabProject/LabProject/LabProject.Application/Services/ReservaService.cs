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

    public ReservaService(
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

    public async Task<bool> CancelarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _reservaRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<ReservaDto> CrearAsync(CreateReservaRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new InvalidOperationException("El usuario no existe.");
        var equipo = await _equipoRepository.GetByIdAsync(request.EquipoId, cancellationToken)
            ?? throw new InvalidOperationException("El equipo no existe.");

        var reserva = new Reserva
        {
            UsuarioId = usuario.Id,
            EquipoId = equipo.Id
        };
        reserva.Programar(request.FechaInicio, request.FechaFin);

        var reservasExistentes = await _reservaRepository.GetReservasDeEquipoEnRangoAsync(equipo.Id, request.FechaInicio, request.FechaFin, cancellationToken);
        reserva.ValidarDisponibilidad(reservasExistentes);

        var softwares = await _softwareRepository.GetByIdsAsync(request.SoftwareIds, cancellationToken);
        if (request.SoftwareIds.Count != softwares.Count)
        {
            throw new InvalidOperationException("Uno o más programas solicitados no existen.");
        }

        foreach (var software in softwares)
        {
            reserva.AgregarSoftware(software);
        }

        var creada = await _reservaRepository.AddAsync(reserva, cancellationToken);
        return Map(creada, usuario, equipo);
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
