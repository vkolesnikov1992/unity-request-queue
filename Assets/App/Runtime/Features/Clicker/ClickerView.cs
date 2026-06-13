using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityRequestQueue.Runtime.Presentation;
using UnityRequestQueue.Runtime.Pooling;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.User;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    [PresenterBinding(presenter: typeof(ClickerPresenter), model: typeof(ClickerModel))]
    public sealed class ClickerView : ViewBase
    {
        [SerializeField]
        private UserResourcesPanel _resourcePanel;
        [SerializeField]
        private Button _collectButton;
        [SerializeField]
        private ClickerCoinBurst _coinBurst;
        
        public UserResourcesPanel ResourcePanel => _resourcePanel;
        public Button CollectButton => _collectButton;
        public RectTransform CoinParticleRoot => _coinBurst.ParticleRoot;
        public int CoinParticlePreloadCount => _coinBurst.PreloadCount;
        public int CoinParticleMaxCount => _coinBurst.PreloadCount * 6;
        public RectTransform CurrencyNotifyRoot => _coinBurst.ParticleRoot;
        public Vector3 CurrencyNotifyStartPosition => _coinBurst.SourceWorldPosition;
        public Sprite CurrencyNotifyIcon => _coinBurst.SourceSprite;
        public Color CurrencyNotifyIconColor => _coinBurst.SourceColor;

        public void InitializeCoinBurst(IComponentPool<ClickerCoinParticle> particlePool)
        {
            _coinBurst.Initialize(particlePool);
        }

        public void PlayCollectButtonPunch(ClickerConfig config)
        {
            var target = _collectButton.transform;
            target.DOKill();
            target.localScale = Vector3.one;
            target.DOPunchScale(
                Vector3.one * config.ButtonPunchScale,
                config.ButtonPunchDurationSeconds,
                config.ButtonPunchVibrato,
                config.ButtonPunchElasticity);
        }

        public void PlayCoinBurst(CancellationToken cancellationToken)
        {
            _coinBurst.PlayAsync(cancellationToken).Forget(Debug.LogException);
        }

        private void OnDestroy()
        {
            _collectButton.transform.DOKill();
        }
    }
}
