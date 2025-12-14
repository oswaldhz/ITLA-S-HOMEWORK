using LabProject.Application.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LabProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Controllers;

[Authorize(Roles = "Admin,Asistente,Estudiante")]
[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;
    private readonly IReservationService _reservationService;

    public ReservasController(IReservaService reservaService, IReservationService reservationService)
    {
        _reservaService = reservaService;
        _reservationService = reservationService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ReservaDto>>> Get([FromQuery] DateTime? fecha, [FromQuery] int? equipoId, [FromQuery] int? usuarioId, CancellationToken cancellationToken)
    {
        var reservas = await _reservaService.ObtenerPorFiltrosAsync(fecha, equipoId, usuarioId, cancellationToken);
        return Ok(reservas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var reserva = await _reservaService.ObtenerPorIdAsync(id, cancellationToken);
        if (reserva == null)
        {
            return NotFound();
        }

        return Ok(reserva);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Asistente,Estudiante")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<ReservaDto>> Post([FromBody] CreateReservaRequest request, CancellationToken cancellationToken)
    {

        // Seguridad: un Estudiante no debería poder crear reservas a nombre de otro usuario.
        var currentUserId = int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var parsedUserId)
            ? parsedUserId
            : 0;

        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        var canCreateForOthers =
            role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("Asistente", StringComparison.OrdinalIgnoreCase);

        if (!canCreateForOthers)
        {
            request.UsuarioId = currentUserId;
        }
        else if (request.UsuarioId <= 0)
        {
            request.UsuarioId = currentUserId;
        }

        if (request.UsuarioId <= 0)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Usuario inválido",
                Detail = "No se pudo determinar el usuario autenticado para crear la reserva."
            });
        }


        var validationResult = await _reservationService.ValidateAsync(request, cancellationToken);
        var creada = await _reservaService.CrearAsync(request, validationResult, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Asistente,Estudiante")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        // Políticas de cancelación:
        // - Admin/Asistente: pueden cancelar cualquier reserva.
        // - Estudiante: solo puede cancelar sus reservas y únicamente si no ha comenzado.

        var reserva = await _reservaService.ObtenerPorIdAsync(id, cancellationToken);
        if (reserva == null)
        {
            return NotFound();
        }

        var currentUserId = int.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var parsedUserId)
            ? parsedUserId
            : 0;

        var role = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        var isStudent = role.Equals("Estudiante", StringComparison.OrdinalIgnoreCase);

        if (isStudent)
        {
            if (currentUserId <= 0 || reserva.UsuarioId != currentUserId)
            {
                return Forbid();
            }

            // Comparación en UTC (el frontend envía ISO UTC). En escenarios reales, se recomienda guardar TZ explícita.
            if (DateTime.UtcNow >= reserva.FechaInicio)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "No se puede cancelar",
                    Detail = "Esta reserva ya comenzó (o está en curso)."
                });
            }
        }

        var cancelada = await _reservaService.CancelarAsync(id, cancellationToken);
        return cancelada ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Asistente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReservaDto>> Put(int id, [FromBody] CreateReservaRequest request, CancellationToken cancellationToken)
    {
        var existing = await _reservaService.ObtenerPorIdAsync(id, cancellationToken);
        if (existing == null)
        {
            return NotFound();
        }

        var validationResult = await _reservationService.ValidateForUpdateAsync(id, request, cancellationToken);
        var updated = await _reservaService.ActualizarAsync(id, request, validationResult, cancellationToken);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
