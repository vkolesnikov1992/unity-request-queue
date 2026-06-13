using System;

namespace UnityRequestQueue.Runtime.Features.Weather.Network
{
    [Serializable]
    internal sealed class WeatherForecastResponse
    {
        public WeatherForecastProperties properties;
    }

    [Serializable]
    internal sealed class WeatherForecastProperties
    {
        public string generatedAt;
        public string updateTime;
        public WeatherForecastPeriod[] periods;
    }

    [Serializable]
    internal sealed class WeatherForecastPeriod
    {
        public string name;
        public int temperature;
        public string temperatureUnit;
        public string icon;
    }
}
