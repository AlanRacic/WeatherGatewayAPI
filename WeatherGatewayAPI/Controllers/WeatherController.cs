using Microsoft.AspNetCore.Mvc;
using WeatherGatewayAPI.Service;
using Microsoft.AspNetCore.Authorization;
using WeatherGatewayAPI.Dtos;

namespace WeatherGatewayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WeatherController : ControllerBase
    {
        private readonly IExternalService _externalService;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IExternalService externalService, ILogger<WeatherController> logger)
        {
            _externalService = externalService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetByCity([FromQuery] string city) 
        {
            _logger.LogInformation("Received weather request. City: {City}", city);

            if (string.IsNullOrWhiteSpace(city)) 
            {
                _logger.LogWarning("Weather request failed validation because string was missing or empty");
                return BadRequest("City is required.");
            }

            city = city.Trim();

            _logger.LogInformation("Validated weather request for city: {City}", city);

            var result = await _externalService.GetWeatherByCityAsync(city);

            if (result.Success)
            {
                _logger.LogInformation("Weather request completed successfully for city: {City}", city);
                return Ok(result.Data); 
            }

            if (result.StatusCode == 404)
            {
                _logger.LogWarning("City not found in external service. City: {City}", city);
                return NotFound(new { message = result.ErrorMessage }); 
            }

            _logger.LogError("Weather request failed for city: {City}. StatusCode: {StatusCode}. Error: {ErrorMessage}", city, result.StatusCode, result.ErrorMessage);
            return StatusCode(502, new {message = result.ErrorMessage ?? "External service returned an error."});
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchByCity([FromBody] WeatherSearchRequest request) 
        {
            if (request == null || string.IsNullOrWhiteSpace(request.City))
            {
                _logger.LogWarning("Weather POST request failed validation because request body or city was missing.");
                return BadRequest(new { message = "City is required." });
            }

            var city = request.City.Trim();

            _logger.LogInformation("Received weather POST request. City:{City}", city);

            var result = await _externalService.GetWeatherByCityAsync(city);

            if (result.Success)
            {
                _logger.LogInformation("Weather POST request completed successfully for city: {City}", city);
                return Ok(result.Data);
            }

            if (result.StatusCode == 404) 
            {
                _logger.LogWarning("City not found in external service. City: {City}", city);
                return NotFound(new {message = result.ErrorMessage});
            }

            return StatusCode(502, new { message = result.ErrorMessage ?? "External service returned an error."});
        }
    }
}
