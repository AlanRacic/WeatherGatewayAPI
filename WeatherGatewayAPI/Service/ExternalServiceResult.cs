using WeatherGatewayAPI.Dtos;

namespace WeatherGatewayAPI.Service
{
    public class ExternalServiceResult
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public WeatherDto? Data { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
