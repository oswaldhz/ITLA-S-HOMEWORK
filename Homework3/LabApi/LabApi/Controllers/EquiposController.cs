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

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EquipoDto>>> GetEquipos()
        {
            var equipos = await _equipoService.GetAllAsync();
            return Ok(equipos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EquipoDto>> GetEquipo(int id)
        {
            var equipo = await _equipoService.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound($"Equipo con ID {id} no encontrado");
            }

            return Ok(equipo);
        }

        [HttpPost]
        public async Task<ActionResult<EquipoDto>> CreateEquipo(CreateEquipoDto createEquipoDto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var equipo = await _equipoService.CreateAsync(createEquipoDto);
            return CreatedAtAction(nameof(GetEquipo), new { id = equipo.Id }, equipo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EquipoDto>> UpdateEquipo(int id, UpdateEquipoDto updateEquipoDto)
        {
            if (id != updateEquipoDto.Id)
            {
                return BadRequest("ID del equipo no coincide");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var equipo = await _equipoService.UpdateAsync(updateEquipoDto);
            if (equipo == null)
            {
                return NotFound($"Equipo con ID {id} no encontrado");
            }

            return Ok(equipo);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEquipo(int id)
        {
            var result = await _equipoService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Equipo con ID {id} no encontrado");
            }

            return NoContent();
        }

        [HttpPost("{id}/reservar")]
        public async Task<ActionResult<EquipoDto>> ReserveEquipo(int id)
        {
            var equipo = await _equipoService.ReserveAsync(id);
            return Ok(equipo);
        }
    }
}
