namespace WeatherGatewayAPI.Dtos
{
    public class ExternalWeatherResponse
    {
        public string Name { get; set; } = string.Empty;
        public MainInfo Main { get; set; } = new();
        public List<WeatherInfo> Weather { get; set; } = new();
    }

    public class MainInfo 
    {
        public double Temp { get; set; }
    }

    public class WeatherInfo
    {
        public string Description { get; set; } = string.Empty;
    }
}
