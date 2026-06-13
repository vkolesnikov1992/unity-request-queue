using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Features.Weather.Network;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.Weather
{
    public sealed class WeatherPresenter : NetworkTabPresenterBase<WeatherView, WeatherParameters, WeatherModel>
    {
        private readonly IRequestQueue _requestQueue;

        private string _cachedIconUrl;
        private Sprite _cachedIconSprite;
        private Texture2D _cachedIconTexture;
        private string _loadingIconUrl;
        private RequestHandle<Texture2D> _loadingIconHandle;

        [Preserve]
        public WeatherPresenter(IRequestQueue requestQueue)
            : base(requestQueue)
        {
            _requestQueue = requestQueue;
        }

        public override TabId TabId => TabId.Weather;

        protected override WeatherParameters DefaultParameters => WeatherParameters.Default;

        protected override UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            var hasCachedForecast = HasCachedForecast();

            if (hasCachedForecast)
            {
                ApplyCachedForecast();
            }

            WeatherLoopAsync(cancellationToken, hasCachedForecast).Forget(Debug.LogException);
            return UniTask.CompletedTask;
        }

        private async UniTask WeatherLoopAsync(
            CancellationToken cancellationToken,
            bool delayFirstRefresh)
        {
            try
            {
                if (delayFirstRefresh)
                {
                    await DelayRefreshIntervalAsync(cancellationToken);
                }

                while (!cancellationToken.IsCancellationRequested)
                {
                    await LoadWeatherAsync(cancellationToken);
                    await DelayRefreshIntervalAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private async UniTask LoadWeatherAsync(CancellationToken cancellationToken)
        {
            var handle = _requestQueue.Enqueue(
                new WeatherForecastRequestCommand(Parameters.ForecastUrl),
                RequestScope);

            try
            {
                var forecast = await handle.Task.AttachExternalCancellation(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                await SetWeatherIconAsync(forecast.IconUrl, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Model.ForecastUrl = Parameters.ForecastUrl;
                Model.IconUrl = forecast.IconUrl;
                Model.TemperatureFahrenheit = forecast.Temperature;
                Model.TemperatureUnit = forecast.TemperatureUnit;
                Model.Error = null;

                View.SetForecast(forecast.Temperature, forecast.TemperatureUnit);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || handle.Status == RequestStatus.Canceled)
            {
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private bool HasCachedForecast()
        {
            return string.Equals(Model.ForecastUrl, Parameters.ForecastUrl, StringComparison.Ordinal) &&
                   !string.IsNullOrWhiteSpace(Model.TemperatureUnit);
        }

        private UniTask DelayRefreshIntervalAsync(CancellationToken cancellationToken)
        {
            return UniTask.Delay(
                TimeSpan.FromSeconds(Mathf.Max(0.01f, Parameters.RefreshIntervalSeconds)),
                cancellationToken: cancellationToken);
        }

        private void ApplyCachedForecast()
        {
            View.SetForecast(Model.TemperatureFahrenheit, Model.TemperatureUnit);

            if (_cachedIconSprite != null)
            {
                View.SetWeatherIcon(_cachedIconSprite);
            }
        }

        protected override void OnDispose()
        {
            CancelIconLoading();
            ClearIconCache();
            base.OnDispose();
        }

        private async UniTask SetWeatherIconAsync(
            string iconUrl,
            CancellationToken cancellationToken)
        {
            if (string.Equals(_cachedIconUrl, iconUrl, StringComparison.Ordinal) && _cachedIconSprite != null)
            {
                View.SetWeatherIcon(_cachedIconSprite);
                return;
            }

            if (string.Equals(_loadingIconUrl, iconUrl, StringComparison.Ordinal) &&
                _loadingIconHandle != null &&
                !_loadingIconHandle.IsCompleted)
            {
                return;
            }

            CancelIconLoading();
            ClearIconCache();

            var handle = _requestQueue.Enqueue(
                new WeatherIconRequestCommand(iconUrl),
                RequestScope);

            _loadingIconUrl = iconUrl;
            _loadingIconHandle = handle;

            var texture = await handle.Task.AttachExternalCancellation(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                UnityEngine.Object.Destroy(texture);
                return;
            }

            if (_loadingIconHandle != handle)
            {
                UnityEngine.Object.Destroy(texture);
                return;
            }

            _cachedIconTexture = texture;
            _cachedIconUrl = iconUrl;
            _cachedIconSprite = Sprite.Create(
                _cachedIconTexture,
                new Rect(0f, 0f, _cachedIconTexture.width, _cachedIconTexture.height),
                new Vector2(0.5f, 0.5f));

            View.SetWeatherIcon(_cachedIconSprite);
            _loadingIconUrl = null;
            _loadingIconHandle = null;
        }

        private void CancelIconLoading()
        {
            if (_loadingIconHandle != null && !_loadingIconHandle.IsCompleted)
            {
                _loadingIconHandle.Cancel();
            }

            _loadingIconHandle = null;
            _loadingIconUrl = null;
        }

        private void ClearIconCache()
        {
            if (_cachedIconSprite)
            {
                UnityEngine.Object.Destroy(_cachedIconSprite);
                _cachedIconSprite = null;
            }

            if (_cachedIconTexture)
            {
                UnityEngine.Object.Destroy(_cachedIconTexture);
                _cachedIconTexture = null;
            }

            _cachedIconUrl = null;
            View.SetWeatherIcon(null);
        }
    }
}
