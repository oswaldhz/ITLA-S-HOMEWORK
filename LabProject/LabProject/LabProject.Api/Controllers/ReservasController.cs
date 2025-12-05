using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IReservaService _reservaService;

    public ReservasController(IReservaService reservaService)
    {
        _reservaService = reservaService;
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
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReservaDto>> Post([FromBody] CreateReservaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var creada = await _reservaService.CrearAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = creada.Id }, creada);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
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
