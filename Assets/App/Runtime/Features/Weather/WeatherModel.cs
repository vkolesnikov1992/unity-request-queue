namespace UnityRequestQueue.Runtime.Features.Weather
{
    public sealed class WeatherModel
    {
        public bool IsLoading { get; set; }

        public string IconUrl { get; set; }

        public int TemperatureFahrenheit { get; set; }

        public string Error { get; set; }
    }
}
