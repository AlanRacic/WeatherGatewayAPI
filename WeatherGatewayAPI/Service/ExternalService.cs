using System.Net;
using System.Text.Json;
using WeatherGatewayAPI.Dtos;
using Microsoft.Extensions.Options;

namespace WeatherGatewayAPI.Service
{
    public class ExternalService : IExternalService
    {
        private readonly HttpClient _httpClient;
        private readonly ExternalWeatherApiSettings _settings;
        private readonly ILogger<ExternalService> _logger;

        public ExternalService(HttpClient httpClient, IOptions<ExternalWeatherApiSettings> options, ILogger<ExternalService> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _logger = logger;
        }

        public async Task<ExternalServiceResult> GetWeatherByCityAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _logger.LogError("API key is missing from configuration.");

                throw new InvalidOperationException("API key is missing from configuration.");
            }

            var requestUrl = $"weather?city={city}&appid={_settings.ApiKey}";

            _logger.LogInformation("Calling external weather API for city: {City}", city);

            var response = await _httpClient.GetAsync(requestUrl);

            _logger.LogInformation("External weather API responded with status code: {StatusCode} for city: {City}", (int)response.StatusCode, city);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ExternalServiceResult
                {
                    Success = false,
                    StatusCode = 404,
                    ErrorMessage = "City not found in external service."
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                return new ExternalServiceResult
                {
                    Success = false,
                    StatusCode = (int)response.StatusCode,
                    ErrorMessage = $"External service returned status code {(int)response.StatusCode}"
                };
            }

            var content = await response.Content.ReadAsStringAsync();


            var externalWeather = JsonSerializer.Deserialize<ExternalWeatherResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (externalWeather == null)
            {
                _logger.LogError("Deserialization failed for city: {City}", city);

                throw new InvalidOperationException("Failed to deserialize external service response.");
            }

            var weatherDto = new WeatherDto
            {
                City = externalWeather.Name,
                Temperature = externalWeather.Main.Temp,
                Description = externalWeather.Weather.FirstOrDefault()?.Description ?? "No description"
            };

            _logger.LogInformation("Successfully mapped external response to WeatherDto for city {City}", city);

            return new ExternalServiceResult
            {
                Success = true,
                StatusCode = (int)response.StatusCode,
                Data = weatherDto
            };
        }
    }
}
