namespace WeatherGatewayAPI.Service
{
    public class ExternalWeatherApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; }
    }
}
