using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class LoadingScreenService : ILoadingScreenService, IDisposable
    {
        private readonly IAssetProvider _assetProvider;
        private readonly IRequestQueue _requestQueue;
        private readonly UICanvasRoot _canvasRoot;
        private readonly HashSet<int> _manualHandles = new();

        private GameObject _screenObject;
        private LoadingScreen _screen;
        private string _manualMessage;
        private string _startupMessage;
        private bool _queueVisible;
        private bool _startupVisible;
        private bool _disposed;
        private int _nextHandleId;

        [Preserve]
        public LoadingScreenService(
            IAssetProvider assetProvider,
            IRequestQueue requestQueue,
            UICanvasRoot canvasRoot)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            _requestQueue = requestQueue ?? throw new ArgumentNullException(nameof(requestQueue));
            _canvasRoot = canvasRoot ?? throw new ArgumentNullException(nameof(canvasRoot));

            _requestQueue.Changed += OnRequestQueueChanged;
            OnRequestQueueChanged();
        }

        public bool IsVisible { get; private set; }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_screen != null)
            {
                UpdateVisibility();
                return;
            }

            try
            {
                _screenObject = await _assetProvider.InstantiateAsync(
                    UIAssetKeys.LoadingScreen,
                    _canvasRoot.Transform,
                    cancellationToken);

                StretchToParent(_screenObject.transform as RectTransform);

                _screen = _screenObject.GetComponentInChildren<LoadingScreen>(true);

                if (_screen == null)
                {
                    throw new InvalidOperationException(
                        $"Loading screen prefab '{UIAssetKeys.LoadingScreen}' does not contain '{nameof(LoadingScreen)}'.");
                }

                _screen.SetVisible(false);
            }
            catch
            {
                ReleaseScreen();
                throw;
            }

            UpdateVisibility();
        }

        public LoadingScreenHandle Show(string message = null)
        {
            ThrowIfDisposed();

            var handle = new LoadingScreenHandle(this, ++_nextHandleId);
            _manualHandles.Add(handle.Id);

            if (!string.IsNullOrWhiteSpace(message))
            {
                _manualMessage = message;
            }

            UpdateVisibility();
            return handle;
        }

        public void Hide(LoadingScreenHandle handle)
        {
            handle?.Dispose();
        }

        public void HideAll()
        {
            ThrowIfDisposed();

            _manualHandles.Clear();
            _manualMessage = null;
            UpdateVisibility();
        }

        public void SetMessage(string message)
        {
            ThrowIfDisposed();

            _manualMessage = string.IsNullOrWhiteSpace(message) ? null : message;

            if (_screen != null)
            {
                _screen.SetMessage(GetMessage());
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _requestQueue.Changed -= OnRequestQueueChanged;
            _manualHandles.Clear();
            ReleaseScreen();
        }

        internal void Release(LoadingScreenHandle handle)
        {
            if (handle == null || _disposed)
            {
                return;
            }

            if (!_manualHandles.Remove(handle.Id))
            {
                return;
            }

            if (_manualHandles.Count == 0)
            {
                _manualMessage = null;
            }

            UpdateVisibility();
        }

        internal void ShowStartup(string message = null)
        {
            ThrowIfDisposed();

            _startupVisible = true;
            _startupMessage = string.IsNullOrWhiteSpace(message) ? null : message;
            UpdateVisibility();
        }

        internal void HideStartup()
        {
            if (_disposed)
            {
                return;
            }

            _startupVisible = false;
            _startupMessage = null;
            UpdateVisibility();
        }

        private void OnRequestQueueChanged()
        {
            if (_disposed)
            {
                return;
            }

            _queueVisible = _requestQueue.LoadingScreenActiveCount > 0;
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            var visible = _startupVisible || _queueVisible || _manualHandles.Count > 0;
            IsVisible = visible;

            if (_screen == null)
            {
                return;
            }

            if (visible)
            {
                _screenObject.transform.SetAsLastSibling();
            }

            _screen.SetMessage(GetMessage());
            _screen.SetVisible(visible);
        }

        private string GetMessage()
        {
            if (!string.IsNullOrWhiteSpace(_manualMessage))
            {
                return _manualMessage;
            }

            return _startupMessage;
        }

        private void ReleaseScreen()
        {
            _screen = null;

            if (_screenObject != null)
            {
                _assetProvider.ReleaseInstance(_screenObject);
                _screenObject = null;
            }
        }

        private static void StretchToParent(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        private void ThrowIfDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(nameof(LoadingScreenService));
        }
    }
}
