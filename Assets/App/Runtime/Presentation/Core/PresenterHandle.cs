using System;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation
{
    public sealed class PresenterHandle : IDisposable
    {
        private readonly IAssetProvider _assetProvider;
        private bool _disposed;

        public PresenterHandle(
            IAssetProvider assetProvider,
            GameObject instance,
            IView view,
            IPresenter presenter)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
            View = view ?? throw new ArgumentNullException(nameof(view));
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }

        public GameObject Instance { get; }

        public IView View { get; }

        public IPresenter Presenter { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Presenter.Dispose();
            _assetProvider.ReleaseInstance(Instance);
        }
    }
}
