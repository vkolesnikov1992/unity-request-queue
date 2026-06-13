namespace UnityRequestQueue.Runtime.Features.Weather
{
    public sealed class WeatherModel
    {
        public string IconUrl { get; set; }

        public string ForecastUrl { get; set; }

        public int TemperatureFahrenheit { get; set; }

        public string TemperatureUnit { get; set; }

        public string Error { get; set; }
    }
}
