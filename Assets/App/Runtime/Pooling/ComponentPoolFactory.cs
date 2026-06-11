using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Pooling
{
    public sealed class ComponentPoolFactory : IComponentPoolFactory
    {
        private readonly IAssetProvider _assetProvider;

        [Preserve]
        public ComponentPoolFactory(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        }

        public async UniTask<IComponentPool<TComponent>> CreateAsync<TComponent>(
            ComponentPoolRequest<TComponent> request,
            CancellationToken cancellationToken)
            where TComponent : Component
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var pool = new AddressableComponentPool<TComponent>(_assetProvider, request);

            try
            {
                await pool.PrewarmAsync(request.PreloadCount, cancellationToken);
                return pool;
            }
            catch
            {
                pool.Dispose();
                throw;
            }
        }
    }
}
