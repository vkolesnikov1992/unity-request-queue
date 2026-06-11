using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityRequestQueue.Runtime.Pooling
{
    public sealed class ComponentPoolAssetKey
    {
        private ComponentPoolAssetKey(object value, string description)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            Description = string.IsNullOrWhiteSpace(description)
                ? value.ToString()
                : description;
        }

        public object Value { get; }

        public string Description { get; }

        public static ComponentPoolAssetKey FromAddressableKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return new ComponentPoolAssetKey(key, key?.ToString());
        }

        public static ComponentPoolAssetKey FromPrefabName(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                throw new ArgumentException("Prefab name cannot be empty.", nameof(prefabName));
            }

            return new ComponentPoolAssetKey(prefabName, prefabName);
        }

        public static ComponentPoolAssetKey FromComponentTypeName<TComponent>()
            where TComponent : Component
        {
            return FromPrefabName(typeof(TComponent).Name);
        }

        public static ComponentPoolAssetKey FromAssetReference(AssetReferenceGameObject assetReference)
        {
            if (assetReference == null)
            {
                throw new ArgumentNullException(nameof(assetReference));
            }

            if (!assetReference.RuntimeKeyIsValid())
            {
                throw new ArgumentException("Asset reference runtime key is invalid.", nameof(assetReference));
            }

            return new ComponentPoolAssetKey(assetReference.RuntimeKey, assetReference.AssetGUID);
        }

        public override string ToString()
        {
            return Description;
        }
    }
}
