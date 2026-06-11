using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Pooling
{
    public interface IComponentPool<TComponent> : IDisposable
        where TComponent : Component
    {
        int CountActive { get; }

        int CountInactive { get; }

        int CountTotal { get; }

        int MaxSize { get; }

        UniTask<TComponent> RentAsync(CancellationToken cancellationToken);

        void Return(TComponent component);

        void ClearInactive();
    }
}
