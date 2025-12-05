using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Controllers;

[Authorize(Roles = "Admin,Asistente,Estudiante")]
[ApiController]
[Route("api/[controller]")]
public class EquiposController : ControllerBase
{
    private readonly IEquipoService _equipoService;

    public EquiposController(IEquipoService equipoService)
    {
        _equipoService = equipoService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EquipoDto>>> Get(CancellationToken cancellationToken)
    {
        var equipos = await _equipoService.ObtenerTodosAsync(cancellationToken);
        return Ok(equipos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var equipo = await _equipoService.ObtenerPorIdAsync(id, cancellationToken);
        if (equipo == null)
        {
            return NotFound();
        }

        return Ok(equipo);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Asistente")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<EquipoDto>> Post([FromBody] SaveEquipoRequest request, CancellationToken cancellationToken)
    {
        var created = await _equipoService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Asistente")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EquipoDto>> Put(int id, [FromBody] SaveEquipoRequest request, CancellationToken cancellationToken)
    {
        var updated = await _equipoService.ActualizarAsync(id, request, cancellationToken);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _equipoService.EliminarAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
