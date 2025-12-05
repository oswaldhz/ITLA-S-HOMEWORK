using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SoftwareController : ControllerBase
{
    private readonly ISoftwareService _softwareService;

    public SoftwareController(ISoftwareService softwareService)
    {
        _softwareService = softwareService;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SoftwareDto>>> Get(CancellationToken cancellationToken)
    {
        var softwares = await _softwareService.ObtenerTodosAsync(cancellationToken);
        return Ok(softwares);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoftwareDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var software = await _softwareService.ObtenerPorIdAsync(id, cancellationToken);
        if (software == null)
        {
            return NotFound();
        }

        return Ok(software);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<SoftwareDto>> Post([FromBody] SaveSoftwareRequest request, CancellationToken cancellationToken)
    {
        var created = await _softwareService.CrearAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SoftwareDto>> Put(int id, [FromBody] SaveSoftwareRequest request, CancellationToken cancellationToken)
    {
        var updated = await _softwareService.ActualizarAsync(id, request, cancellationToken);
        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _softwareService.EliminarAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
