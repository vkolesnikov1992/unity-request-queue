using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Pooling
{
    internal sealed class AddressableComponentPool<TComponent> : IComponentPool<TComponent>
        where TComponent : Component
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ComponentPoolRequest<TComponent> _request;
        private readonly Queue<TComponent> _inactive = new();
        private readonly HashSet<TComponent> _inactiveSet = new();
        private readonly HashSet<TComponent> _active = new();
        private readonly Dictionary<TComponent, GameObject> _instances = new();

        private int _pendingCreates;
        private bool _disposed;

        public AddressableComponentPool(
            IAssetProvider assetProvider,
            ComponentPoolRequest<TComponent> request)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            _request = request ?? throw new ArgumentNullException(nameof(request));
        }

        public int CountActive => _active.Count;

        public int CountInactive => _inactive.Count;

        public int CountTotal => _instances.Count;

        public int MaxSize => _request.MaxSize;

        public async UniTask<TComponent> RentAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            TComponent component;
            if (_inactive.Count > 0)
            {
                component = _inactive.Dequeue();
                _inactiveSet.Remove(component);
            }
            else
            {
                component = await CreateReservedComponentAsync(cancellationToken);
            }

            _active.Add(component);
            var instance = _instances[component];
            instance.SetActive(true);

            try
            {
                NotifyRent(component);
            }
            catch
            {
                _active.Remove(component);
                _instances.Remove(component);
                _assetProvider.ReleaseInstance(instance);
                throw;
            }

            return component;
        }

        public void Return(TComponent component)
        {
            if (component == null)
            {
                return;
            }

            ThrowIfDisposed();

            if (!_instances.TryGetValue(component, out var instance))
            {
                throw new InvalidOperationException(
                    $"Component '{component.GetType().Name}' does not belong to this pool.");
            }

            if (!_active.Contains(component))
            {
                if (_request.CollectionCheck)
                {
                    throw new InvalidOperationException(
                        $"Component '{component.GetType().Name}' is not rented from this pool.");
                }

                return;
            }

            NotifyReturn(component);
            _active.Remove(component);

            if (!_inactiveSet.Add(component))
            {
                if (_request.CollectionCheck)
                {
                    throw new InvalidOperationException(
                        $"Component '{component.GetType().Name}' has already been returned to this pool.");
                }

                return;
            }

            instance.SetActive(false);
            _inactive.Enqueue(component);
        }

        public void ClearInactive()
        {
            ThrowIfDisposed();
            ReleaseInactive();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            ReleaseAll();
        }

        public async UniTask PrewarmAsync(int count, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            for (int i = 0; i < count; i++)
            {
                var component = await CreateReservedComponentAsync(cancellationToken);
                ReturnCreatedInactive(component);
            }
        }

        private async UniTask<TComponent> CreateReservedComponentAsync(CancellationToken cancellationToken)
        {
            ReserveCreateSlot();

            try
            {
                return await CreateComponentAsync(cancellationToken);
            }
            finally
            {
                _pendingCreates--;
            }
        }

        private void ReserveCreateSlot()
        {
            if (_instances.Count + _pendingCreates >= _request.MaxSize)
            {
                throw new InvalidOperationException(
                    $"Pool '{_request.AssetKey}' reached max size {_request.MaxSize}.");
            }

            _pendingCreates++;
        }

        private async UniTask<TComponent> CreateComponentAsync(CancellationToken cancellationToken)
        {
            var instance = await _assetProvider.InstantiateAsync(
                _request.AssetKey.Value,
                _request.Parent,
                cancellationToken,
                _request.InstantiateInWorldSpace);

            if (_disposed)
            {
                _assetProvider.ReleaseInstance(instance);
                throw new ObjectDisposedException(GetType().Name);
            }

            var component = instance.GetComponentInChildren<TComponent>(true);
            if (component == null)
            {
                _assetProvider.ReleaseInstance(instance);
                throw new InvalidOperationException(
                    $"Pool prefab '{_request.AssetKey}' does not contain component '{typeof(TComponent).Name}'.");
            }

            _instances.Add(component, instance);
            return component;
        }

        private void ReturnCreatedInactive(TComponent component)
        {
            NotifyReturn(component);
            _instances[component].SetActive(false);
            _inactiveSet.Add(component);
            _inactive.Enqueue(component);
        }

        private void ReleaseInactive()
        {
            while (_inactive.Count > 0)
            {
                var component = _inactive.Dequeue();
                _inactiveSet.Remove(component);

                if (!_instances.TryGetValue(component, out var instance))
                {
                    continue;
                }

                _instances.Remove(component);
                _assetProvider.ReleaseInstance(instance);
            }
        }

        private void ReleaseAll()
        {
            foreach (var instance in _instances.Values)
            {
                _assetProvider.ReleaseInstance(instance);
            }

            _instances.Clear();
            _active.Clear();
            _inactive.Clear();
            _inactiveSet.Clear();
        }

        private void NotifyRent(TComponent component)
        {
            _request.OnRent?.Invoke(component);

            if (component is IPoolableComponent poolable)
            {
                poolable.OnRentFromPool();
            }
        }

        private void NotifyReturn(TComponent component)
        {
            _request.OnReturn?.Invoke(component);

            if (component is IPoolableComponent poolable)
            {
                poolable.OnReturnToPool();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
