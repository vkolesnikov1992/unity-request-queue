using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using R3;
using UnityEngine;
using UnityRequestQueue.Runtime.Features.Clicker;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Factories;
using UnityRequestQueue.Runtime.Presentation;

namespace UnityRequestQueue.Runtime.Features.Main
{
    public sealed class MainPresenter : PresenterBase<MainView, MainParameters>
    {
        private readonly IAsyncFactory<PresenterRequest, PresenterHandle> _presenterFactory;
        private readonly ClickerConfig _clickerConfig;

        private PresenterHandle _clickerHandle;
        private IDisposable _tabsSubscription;
        private TabId _activeTab;
        private bool _hasActiveTab;

        public MainPresenter(
            IAsyncFactory<PresenterRequest, PresenterHandle> presenterFactory,
            ClickerConfig clickerConfig)
        {
            _presenterFactory = presenterFactory;
            _clickerConfig = clickerConfig;
        }

        protected override MainParameters DefaultParameters => MainParameters.Default;

        protected override async UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            View.Tabs.Initialize(Parameters.InitialTab);

            await CreateClickerAsync(cancellationToken);

            _tabsSubscription = View.Tabs.Selected.Subscribe(
                this,
                static (tabId, presenter) =>
                    presenter.SwitchToTabAsync(tabId, presenter.LifetimeToken).Forget(Debug.LogException));

            await SwitchToTabAsync(Parameters.InitialTab, cancellationToken);
        }

        protected override async UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            DisposeTabsSubscription();

            if (_hasActiveTab && _activeTab == TabId.Clicker)
            {
                await _clickerHandle.Presenter.ExitAsync(cancellationToken);
            }

            _hasActiveTab = false;
        }

        protected override void OnDispose()
        {
            DisposeTabsSubscription();
            _clickerHandle?.Dispose();
            _clickerHandle = null;
        }

        private async UniTask CreateClickerAsync(CancellationToken cancellationToken)
        {
            var request = new PresenterRequest(
                FeatureAssetKeys.Clicker,
                View.ViewContainer,
                new ClickerParameters(_clickerConfig));

            _clickerHandle = await _presenterFactory.CreateAsync(request, cancellationToken);
            _clickerHandle.View.SetVisible(false);
        }

        private async UniTask SwitchToTabAsync(TabId tabId, CancellationToken cancellationToken)
        {
            if (_hasActiveTab && _activeTab == tabId)
            {
                return;
            }

            if (_hasActiveTab && _activeTab == TabId.Clicker)
            {
                await _clickerHandle.Presenter.ExitAsync(cancellationToken);
            }

            _activeTab = tabId;
            _hasActiveTab = true;

            if (tabId == TabId.Clicker)
            {
                await _clickerHandle.Presenter.EnterAsync(cancellationToken);
            }
        }

        private void DisposeTabsSubscription()
        {
            _tabsSubscription?.Dispose();
            _tabsSubscription = null;
        }
    }
}
