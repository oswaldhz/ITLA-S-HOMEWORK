using LabProject.Application.DTOs;
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
        var validationResult = await _reservationService.ValidateAsync(request, cancellationToken);
        var creada = await _reservaService.CrearAsync(request, validationResult, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Asistente")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var cancelada = await _reservaService.CancelarAsync(id, cancellationToken);
        if (!cancelada)
        {
            return NotFound();
        }

        return NoContent();
    }
}
