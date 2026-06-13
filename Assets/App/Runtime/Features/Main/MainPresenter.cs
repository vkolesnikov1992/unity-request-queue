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
        private PresenterHandle _weatherHandle;
        private PresenterHandle _dogBreedsHandle;
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
            await CreateWeatherAsync(cancellationToken);
            await CreateDogBreedsAsync(cancellationToken);

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
            else if (_hasActiveTab && _activeTab == TabId.Weather)
            {
                await _weatherHandle.Presenter.ExitAsync(cancellationToken);
            }
            else if (_hasActiveTab && _activeTab == TabId.DogBreeds)
            {
                await _dogBreedsHandle.Presenter.ExitAsync(cancellationToken);
            }

            _hasActiveTab = false;
        }

        protected override void OnDispose()
        {
            DisposeTabsSubscription();
            _clickerHandle?.Dispose();
            _weatherHandle?.Dispose();
            _dogBreedsHandle?.Dispose();
            _clickerHandle = null;
            _weatherHandle = null;
            _dogBreedsHandle = null;
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

        private async UniTask CreateWeatherAsync(CancellationToken cancellationToken)
        {
            var request = new PresenterRequest(
                FeatureAssetKeys.Weather,
                View.ViewContainer);

            _weatherHandle = await _presenterFactory.CreateAsync(request, cancellationToken);
            _weatherHandle.View.SetVisible(false);
        }

        private async UniTask CreateDogBreedsAsync(CancellationToken cancellationToken)
        {
            var request = new PresenterRequest(
                FeatureAssetKeys.DogBreeds,
                View.ViewContainer);

            _dogBreedsHandle = await _presenterFactory.CreateAsync(request, cancellationToken);
            _dogBreedsHandle.View.SetVisible(false);
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
                _clickerHandle.View.SetVisible(false);
            }
            else if (_hasActiveTab && _activeTab == TabId.Weather)
            {
                await _weatherHandle.Presenter.ExitAsync(cancellationToken);
                _weatherHandle.View.SetVisible(false);
            }
            else if (_hasActiveTab && _activeTab == TabId.DogBreeds)
            {
                await _dogBreedsHandle.Presenter.ExitAsync(cancellationToken);
                _dogBreedsHandle.View.SetVisible(false);
            }

            _activeTab = tabId;
            _hasActiveTab = true;

            if (tabId == TabId.Clicker)
            {
                _clickerHandle.View.SetVisible(true);
                await _clickerHandle.Presenter.EnterAsync(cancellationToken);
            }
            else if (tabId == TabId.Weather)
            {
                _weatherHandle.View.SetVisible(true);
                await _weatherHandle.Presenter.EnterAsync(cancellationToken);
            }
            else if (tabId == TabId.DogBreeds)
            {
                _dogBreedsHandle.View.SetVisible(true);
                await _dogBreedsHandle.Presenter.EnterAsync(cancellationToken);
            }
        }

        private void DisposeTabsSubscription()
        {
            _tabsSubscription?.Dispose();
            _tabsSubscription = null;
        }
    }
}
