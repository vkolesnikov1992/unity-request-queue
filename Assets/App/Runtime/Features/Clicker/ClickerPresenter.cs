using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Audio;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Pooling;
using UnityRequestQueue.Runtime.User;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerPresenter : TabPresenterBase<ClickerView, ClickerParameters, ClickerModel>
    {
        private readonly UserSection _userSection;
        private readonly IAudioService _audioService;
        private readonly IComponentPoolFactory _componentPoolFactory;

        private IComponentPool<ClickerCoinParticle> _coinParticlePool;
        private IComponentPool<ClickerCurrencyNotify> _currencyNotifyPool;
        private bool _loopsStarted;

        [Preserve]
        public ClickerPresenter(
            UserSection userSection,
            IAudioService audioService,
            IComponentPoolFactory componentPoolFactory)
        {
            _userSection = userSection;
            _audioService = audioService;
            _componentPoolFactory = componentPoolFactory;
        }

        public override TabId TabId => TabId.Clicker;

        protected override void OnAttached(ClickerView view)
        {
            View.ResourcePanel.Initialize(_userSection.Resources);
            View.CollectButton.onClick.AddListener(OnCollectButtonClicked);

            SyncModelWithResources();
            StartLoops();
        }

        protected override UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            return CreateFeedbackPoolsAsync(cancellationToken);
        }

        protected override void OnDispose()
        {
            View.CollectButton.onClick.RemoveListener(OnCollectButtonClicked);
            _coinParticlePool?.Dispose();
            _currencyNotifyPool?.Dispose();
            _coinParticlePool = null;
            _currencyNotifyPool = null;
            base.OnDispose();
        }

        private void OnCollectButtonClicked()
        {
            if (TryCollect())
            {
                PlayCollectFeedback();
            }
        }

        private void StartLoops()
        {
            if (_loopsStarted)
            {
                return;
            }

            _loopsStarted = true;
            AutoCollectLoopAsync(LifetimeToken).Forget(Debug.LogException);
            EnergyRestoreLoopAsync(LifetimeToken).Forget(Debug.LogException);
        }

        private async UniTask AutoCollectLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(GetPositiveInterval(Parameters.Config.AutoCollectIntervalSeconds)),
                        cancellationToken: cancellationToken);

                    if (!IsActive && !Parameters.Config.AutoCollectWhenInactive)
                    {
                        continue;
                    }

                    if (TryCollect() && IsActive)
                    {
                        PlayCollectFeedback();
                    }
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private async UniTask EnergyRestoreLoopAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(GetPositiveInterval(Parameters.Config.EnergyRestoreIntervalSeconds)),
                        cancellationToken: cancellationToken);

                    RestoreEnergy();
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private bool TryCollect()
        {
            var config = Parameters.Config;
            var resources = _userSection.Resources;

            if (!resources.Energy.TrySpend(Math.Max(0, config.EnergyCostPerClick)))
            {
                return false;
            }

            resources.Currency.Add(Math.Max(0, config.CurrencyPerClick));
            SyncModelWithResources();

            return true;
        }

        private void RestoreEnergy()
        {
            _userSection.Resources.Energy.Add(Math.Max(0, Parameters.Config.EnergyRestoreAmount));
            SyncModelWithResources();
        }

        private void SyncModelWithResources()
        {
            var resources = _userSection.Resources;
            Model.Currency = resources.Currency.Amount.Value;
            Model.Energy = resources.Energy.Amount.Value;
        }

        private void PlayCollectFeedback()
        {
            PlayParticleBurst();
            PlayCurrencyFlyAnimation();
            PlayButtonPressAnimation();
            PlayClickSound();
        }

        private void PlayParticleBurst()
        {
            View.PlayCoinBurst(LifetimeToken);
        }

        private void PlayCurrencyFlyAnimation()
        {
            if (_currencyNotifyPool == null)
            {
                return;
            }

            PlayCurrencyFlyAnimationAsync(LifetimeToken).Forget(Debug.LogException);
        }

        private void PlayButtonPressAnimation()
        {
            View.PlayCollectButtonPunch(Parameters.Config);
        }

        private void PlayClickSound()
        {
            if (string.IsNullOrWhiteSpace(Parameters.Config.ClickSoundKey))
            {
                return;
            }

            _audioService
                .PlaySoundEffectAsync(Parameters.Config.ClickSoundKey, LifetimeToken)
                .Forget(Debug.LogException);
        }

        private static float GetPositiveInterval(float intervalSeconds)
        {
            return Mathf.Max(0.01f, intervalSeconds);
        }

        private async UniTask CreateFeedbackPoolsAsync(CancellationToken cancellationToken)
        {
            await CreateCoinParticlePoolAsync(cancellationToken);
            await CreateCurrencyNotifyPoolAsync(cancellationToken);
        }

        private async UniTask CreateCoinParticlePoolAsync(CancellationToken cancellationToken)
        {
            if (_coinParticlePool != null)
            {
                return;
            }

            var request = new ComponentPoolRequest<ClickerCoinParticle>(
                ComponentPoolAssetKey.FromAddressableKey(FeatureAssetKeys.ClickerCoinParticle),
                View.CoinParticleRoot,
                preloadCount: View.CoinParticlePreloadCount,
                maxSize: View.CoinParticleMaxCount);

            _coinParticlePool = await _componentPoolFactory.CreateAsync(request, cancellationToken);
            View.InitializeCoinBurst(_coinParticlePool);
        }

        private async UniTask CreateCurrencyNotifyPoolAsync(CancellationToken cancellationToken)
        {
            if (_currencyNotifyPool != null)
            {
                return;
            }

            var config = Parameters.Config;
            var preloadCount = Mathf.Max(0, config.CurrencyNotifyPreloadCount);
            var maxCount = Math.Max(1, Math.Max(config.CurrencyNotifyMaxCount, preloadCount));
            var request = new ComponentPoolRequest<ClickerCurrencyNotify>(
                ComponentPoolAssetKey.FromAddressableKey(FeatureAssetKeys.ClickerCurrencyNotify),
                View.CurrencyNotifyRoot,
                preloadCount,
                maxCount);

            _currencyNotifyPool = await _componentPoolFactory.CreateAsync(request, cancellationToken);
        }

        private async UniTask PlayCurrencyFlyAnimationAsync(CancellationToken cancellationToken)
        {
            try
            {
                var notify = await _currencyNotifyPool.RentAsync(cancellationToken);
                notify.Play(
                    View.CurrencyNotifyStartPosition,
                    View.CurrencyNotifyIcon,
                    View.CurrencyNotifyIconColor,
                    $"+{Math.Max(0, Parameters.Config.CurrencyPerClick)}",
                    CreateCurrencyNotifyMotion(),
                    _currencyNotifyPool.Return);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
            catch (InvalidOperationException exception)
            {
                Debug.LogWarning(exception.Message);
            }
        }

        private ClickerCurrencyNotifyMotion CreateCurrencyNotifyMotion()
        {
            var config = Parameters.Config;
            var horizontalScatter = Mathf.Max(0f, config.CurrencyNotifyHorizontalScatter);
            var horizontalOffset = UnityEngine.Random.Range(-horizontalScatter, horizontalScatter);

            return new ClickerCurrencyNotifyMotion(
                Vector2.zero,
                new Vector2(horizontalOffset, Mathf.Max(0f, config.CurrencyNotifyVerticalDistance)),
                Mathf.Max(0.01f, config.CurrencyNotifyStartScale),
                Mathf.Max(0.01f, config.CurrencyNotifyEndScale),
                Mathf.Max(0.01f, config.CurrencyNotifyDurationSeconds),
                Mathf.Clamp01(config.CurrencyNotifyFadeDurationPercent));
        }
    }
}
