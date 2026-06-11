namespace UnityRequestQueue.Runtime.Features.Weather
{
    public sealed class WeatherParameters
    {
        public static WeatherParameters Default { get; } = new WeatherParameters(
            "https://api.weather.gov/gridpoints/TOP/32,81/forecast",
            5f);

        public WeatherParameters(string forecastUrl, float refreshIntervalSeconds)
        {
            ForecastUrl = forecastUrl;
            RefreshIntervalSeconds = refreshIntervalSeconds;
        }

        public string ForecastUrl { get; }

        public float RefreshIntervalSeconds { get; }
    }
}
