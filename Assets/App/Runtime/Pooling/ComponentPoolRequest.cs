using System;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Pooling
{
    public sealed class ComponentPoolRequest<TComponent>
        where TComponent : Component
    {
        public ComponentPoolRequest(
            ComponentPoolAssetKey assetKey,
            Transform parent,
            int preloadCount = 0,
            int maxSize = int.MaxValue,
            bool instantiateInWorldSpace = false,
            bool collectionCheck = true,
            Action<TComponent> onRent = null,
            Action<TComponent> onReturn = null)
        {
            if (preloadCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(preloadCount));
            }

            if (maxSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSize));
            }

            if (preloadCount > maxSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(preloadCount),
                    "Preload count cannot be greater than max pool size.");
            }

            AssetKey = assetKey ?? throw new ArgumentNullException(nameof(assetKey));
            Parent = parent;
            PreloadCount = preloadCount;
            MaxSize = maxSize;
            InstantiateInWorldSpace = instantiateInWorldSpace;
            CollectionCheck = collectionCheck;
            OnRent = onRent;
            OnReturn = onReturn;
        }

        public ComponentPoolAssetKey AssetKey { get; }

        public Transform Parent { get; }

        public int PreloadCount { get; }

        public int MaxSize { get; }

        public bool InstantiateInWorldSpace { get; }

        public bool CollectionCheck { get; }

        public Action<TComponent> OnRent { get; }

        public Action<TComponent> OnReturn { get; }
    }
}
