using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Factories
{
    public interface IAsyncFactory<in TParam, TResult>
    {
        UniTask<TResult> CreateAsync(
            TParam param,
            CancellationToken cancellationToken);
    }
}
