using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.Weather.Network
{
    internal sealed class WeatherIconRequestCommand : IRequestCommand<Texture2D>
    {
        private static readonly IReadOnlyDictionary<string, string> SHeaders =
            new Dictionary<string, string>
            {
                { "User-Agent", "unity-request-queue/1.0" }
            };

        private readonly string _url;

        public WeatherIconRequestCommand(string url)
        {
            _url = url;
        }

        public string Name => $"Weather icon {_url}";

        public async UniTask<Texture2D> ExecuteAsync(
            IHttpClient httpClient,
            CancellationToken cancellationToken)
        {
            var response = await httpClient.SendAsync(
                HttpRequest.Get(_url, SHeaders),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Weather icon request failed with status code {response.StatusCode}.");
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            if (!texture.LoadImage(response.Data))
            {
                UnityEngine.Object.Destroy(texture);
                throw new InvalidOperationException("Weather icon response is not a supported image.");
            }

            texture.name = _url;
            return texture;
        }
    }
}
