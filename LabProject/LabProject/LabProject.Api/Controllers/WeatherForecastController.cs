using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabProject.Api.Controllers
{
    [Authorize(Roles = "Admin,Asistente,Estudiante")]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(IWeatherForecastService weatherForecastService)
        {
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecastDto>> Get(CancellationToken cancellationToken)
        {
            return await _weatherForecastService.GetForecastsAsync(cancellationToken);
        }
    }
}
