using TMPro;
using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.Weather
{
    [PresenterBinding(presenter: typeof(WeatherPresenter), model: typeof(WeatherModel))]
    public sealed class WeatherView : ViewBase
    {
        [SerializeField]
        private Image _weatherIcon;
        [SerializeField]
        private TextMeshProUGUI _weatherText;

        public void SetForecast(int temperature, string temperatureUnit)
        {
            _weatherText.text = $"Сегодня - {temperature}{temperatureUnit}";
        }

        public void SetWeatherIcon(Sprite icon)
        {
            _weatherIcon.sprite = icon;
            _weatherIcon.enabled = icon != null;
        }
    }
}
