using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Pooling
{
    public interface IComponentPoolFactory
    {
        UniTask<IComponentPool<TComponent>> CreateAsync<TComponent>(
            ComponentPoolRequest<TComponent> request,
            CancellationToken cancellationToken)
            where TComponent : Component;
    }
}
