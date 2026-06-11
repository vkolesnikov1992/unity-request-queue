using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    [PresenterBinding(presenter: typeof(ClickerPresenter), model: typeof(ClickerModel))]
    public sealed class ClickerView : ViewBase
    {
        [SerializeField]
        private Button _collectButton;
        [SerializeField]
        private ParticleSystem _clickParticles;
        [SerializeField]
        private RectTransform _currencyFlyRoot;
        [SerializeField]
        private AudioSource _audioSource;

        public Button CollectButton => _collectButton;
        public ParticleSystem ClickParticles => _clickParticles;
        public RectTransform CurrencyFlyRoot => _currencyFlyRoot;
        public AudioSource AudioSource => _audioSource;
    }
}
