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
        private Text _weatherText;
        [SerializeField]
        private GameObject _loader;
        [SerializeField]
        private Text _errorText;

        public Image WeatherIcon => _weatherIcon;
        public Text WeatherText => _weatherText;
        public GameObject Loader => _loader;
        public Text ErrorText => _errorText;
    }
}
