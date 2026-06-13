using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.Weather.Network
{
    internal sealed class WeatherForecastRequestCommand : IRequestCommand<WeatherForecast>
    {
        private static readonly IReadOnlyDictionary<string, string> SHeaders =
            new Dictionary<string, string>
            {
                { "User-Agent", "unity-request-queue/1.0" },
                { "Accept", "application/geo+json" },
                { "Cache-Control", "no-cache" }
            };

        private readonly string _url;

        public WeatherForecastRequestCommand(string url)
        {
            _url = url;
        }

        public string Name => $"Weather forecast {_url}";

        public async UniTask<WeatherForecast> ExecuteAsync(
            IHttpClient httpClient,
            CancellationToken cancellationToken)
        {
            var response = await httpClient.SendAsync(
                HttpRequest.Get(_url, SHeaders),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Weather request failed with status code {response.StatusCode}.");
            }

            var forecastResponse = JsonUtility.FromJson<WeatherForecastResponse>(response.Text);
            var periods = forecastResponse?.properties?.periods;
            if (periods == null || periods.Length == 0)
            {
                throw new InvalidOperationException("Weather response does not contain forecast periods.");
            }

            var period = periods[0];

            return new WeatherForecast(
                period.temperature,
                period.temperatureUnit,
                period.icon,
                period.name,
                forecastResponse.properties.generatedAt,
                forecastResponse.properties.updateTime);
        }
    }
}
