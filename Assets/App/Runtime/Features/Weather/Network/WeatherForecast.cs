namespace UnityRequestQueue.Runtime.Features.Weather.Network
{
    internal readonly struct WeatherForecast
    {
        public WeatherForecast(
            int temperature,
            string temperatureUnit,
            string iconUrl,
            string periodName,
            string generatedAt,
            string updateTime)
        {
            Temperature = temperature;
            TemperatureUnit = temperatureUnit;
            IconUrl = iconUrl;
            PeriodName = periodName;
            GeneratedAt = generatedAt;
            UpdateTime = updateTime;
        }

        public int Temperature { get; }

        public string TemperatureUnit { get; }

        public string IconUrl { get; }

        public string PeriodName { get; }

        public string GeneratedAt { get; }

        public string UpdateTime { get; }
    }
}
