using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace UnityRequestQueue.Runtime.AssetManagement
{
    public interface IAssetProvider
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);

        UniTask<TAsset> LoadAsync<TAsset>(
            object key,
            CancellationToken cancellationToken)
            where TAsset : Object;

        UniTask<GameObject> InstantiateAsync(
            object key,
            Transform parent,
            CancellationToken cancellationToken,
            bool instantiateInWorldSpace = false);

        UniTask<GameObject> InstantiateAsync(
            object key,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            CancellationToken cancellationToken);

        void Release<TAsset>(TAsset asset)
            where TAsset : Object;

        bool ReleaseInstance(GameObject instance);
    }
}
