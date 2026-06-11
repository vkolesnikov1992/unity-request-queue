using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityRequestQueue.Runtime.AssetManagement
{
    public sealed class AddressablesAssetProvider : IAssetProvider
    {
        private bool _initialized;

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                return;
            }

            var handle = Addressables.InitializeAsync(true);

            try
            {
                await handle.ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true);

                _initialized = true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException("Addressables initialization failed.", exception);
            }
        }

        public async UniTask<TAsset> LoadAsync<TAsset>(
            object key,
            CancellationToken cancellationToken)
            where TAsset : UnityEngine.Object
        {
            ThrowIfNotInitialized();
            ValidateKey(key);

            var handle = Addressables.LoadAssetAsync<TAsset>(key);

            try
            {
                await handle.ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true);

                ValidateCompleted(handle, key, "load");
                return handle.Result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ReleaseHandleIfValid(handle);
                throw CreateException(key, "load", exception);
            }
        }

        public async UniTask<GameObject> InstantiateAsync(
            object key,
            Transform parent,
            CancellationToken cancellationToken,
            bool instantiateInWorldSpace = false)
        {
            ThrowIfNotInitialized();
            ValidateKey(key);

            var handle = Addressables.InstantiateAsync(
                key,
                parent,
                instantiateInWorldSpace);

            try
            {
                await handle.ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true);

                ValidateCompleted(handle, key, "instantiate");
                return handle.Result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ReleaseHandleIfValid(handle);
                throw CreateException(key, "instantiate", exception);
            }
        }

        public async UniTask<GameObject> InstantiateAsync(
            object key,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            CancellationToken cancellationToken)
        {
            ThrowIfNotInitialized();
            ValidateKey(key);

            var handle = Addressables.InstantiateAsync(
                key,
                position,
                rotation,
                parent);

            try
            {
                await handle.ToUniTask(
                    cancellationToken: cancellationToken,
                    cancelImmediately: true,
                    autoReleaseWhenCanceled: true);

                ValidateCompleted(handle, key, "instantiate");
                return handle.Result;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                ReleaseHandleIfValid(handle);
                throw CreateException(key, "instantiate", exception);
            }
        }

        public void Release<TAsset>(TAsset asset)
            where TAsset : UnityEngine.Object
        {
            if (asset == null)
            {
                return;
            }

            Addressables.Release(asset);
        }

        public bool ReleaseInstance(GameObject instance)
        {
            return instance != null && Addressables.ReleaseInstance(instance);
        }

        private void ThrowIfNotInitialized()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException(
                    "AddressablesAssetProvider is not initialized. Run asset startup step before loading assets.");
            }
        }

        private static void ValidateKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
        }

        private static void ValidateCompleted<TAsset>(
            AsyncOperationHandle<TAsset> handle,
            object key,
            string operation)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return;
            }

            throw CreateException(key, operation, handle.OperationException);
        }

        private static InvalidOperationException CreateException(
            object key,
            string operation,
            Exception innerException)
        {
            return new InvalidOperationException(
                $"Addressables asset {operation} failed. Key: {key}",
                innerException);
        }

        private static void ReleaseHandleIfValid<TAsset>(AsyncOperationHandle<TAsset> handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
