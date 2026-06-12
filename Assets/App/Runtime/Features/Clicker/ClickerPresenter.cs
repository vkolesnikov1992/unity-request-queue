using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.User;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerPresenter : TabPresenterBase<ClickerView, ClickerParameters, ClickerModel>
    {
        private readonly UserSection _userSection;

        private bool _loopsStarted;

        [Preserve]
        public ClickerPresenter(UserSection userSection)
        {
            _userSection = userSection;
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
            return UniTask.CompletedTask;
        }

        protected override void OnDispose()
        {
            View.CollectButton.onClick.RemoveListener(OnCollectButtonClicked);
            base.OnDispose();
        }

        private void OnCollectButtonClicked()
        {
            TryCollect(silent: false);
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

                    TryCollect(silent: !IsActive);
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

        private bool TryCollect(bool silent)
        {
            var config = Parameters.Config;
            var resources = _userSection.Resources;

            if (!resources.Energy.TrySpend(Math.Max(0, config.EnergyCostPerClick)))
            {
                return false;
            }

            resources.Currency.Add(Math.Max(0, config.CurrencyPerClick));
            SyncModelWithResources();

            if (!silent)
            {
                PlayCollectFeedback();
            }

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
        }

        private void PlayCurrencyFlyAnimation()
        {
        }

        private void PlayButtonPressAnimation()
        {
        }

        private void PlayClickSound()
        {
        }

        private static float GetPositiveInterval(float intervalSeconds)
        {
            return Mathf.Max(0.01f, intervalSeconds);
        }
    }
}
