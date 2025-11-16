using Microsoft.AspNetCore.Mvc;
using LaboratorioAPI.Interfaces;
using LaboratorioAPI.DTOs;

namespace LaboratorioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquiposController : ControllerBase
    {
        private readonly IEquipoService _equipoService;
        private readonly ILogger<EquiposController> _logger;

        public EquiposController(IEquipoService equipoService, ILogger<EquiposController> logger)
        {
            _equipoService = equipoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoDto>>> GetEquipos()
        {
            try
            {
                var equipos = await _equipoService.GetAllAsync();
                return Ok(equipos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDto>> GetEquipo(int id)
        {
            try
            {
                var equipo = await _equipoService.GetByIdAsync(id);
                if (equipo == null)
                {
                    return NotFound($"Equipo con ID {id} no encontrado");
                }
                return Ok(equipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener equipo con ID {EquipoId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<EquipoDto>> CreateEquipo(CreateEquipoDto createEquipoDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var equipo = await _equipoService.CreateAsync(createEquipoDto);
                return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear equipo");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EquipoDto>> UpdateEquipo(int id, UpdateEquipoDto updateEquipoDto)
        {
            try
            {
                if (id != updateEquipoDto.Id)
                {
                    return BadRequest("ID del equipo no coincide");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var equipo = await _equipoService.UpdateAsync(updateEquipoDto);
                if (equipo == null)
                {
                    return NotFound($"Equipo con ID {id} no encontrado");
                }

                return Ok(equipo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar equipo con ID {EquipoId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            try
            {
                var result = await _equipoService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound($"Equipo con ID {id} no encontrado");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar equipo con ID {EquipoId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}