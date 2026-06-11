using System;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation
{
    public sealed class PresenterRequest
    {
        public PresenterRequest(
            object assetKey,
            Transform parent,
            object parameters = null,
            bool instantiateInWorldSpace = false)
            : this(assetKey, parent, parameters, null, instantiateInWorldSpace)
        {
        }

        public PresenterRequest(
            object assetKey,
            Transform parent,
            object parameters,
            object model,
            bool instantiateInWorldSpace = false)
        {
            AssetKey = assetKey ?? throw new ArgumentNullException(nameof(assetKey));
            Parent = parent;
            Parameters = parameters;
            Model = model;
            InstantiateInWorldSpace = instantiateInWorldSpace;
        }

        public object AssetKey { get; }

        public Transform Parent { get; }

        public object Parameters { get; }

        public object Model { get; }

        public bool InstantiateInWorldSpace { get; }
    }
}
