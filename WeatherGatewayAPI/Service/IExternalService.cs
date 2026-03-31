namespace WeatherGatewayAPI.Service
{
    public interface IExternalService
    {
        Task<ExternalServiceResult> GetWeatherByCityAsync(string city);
    }
}
