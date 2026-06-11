using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Network
{
    public interface IRequestCommand
    {
        string Name { get; }
    }

    public interface IRequestCommand<TResponse> : IRequestCommand
    {
        UniTask<TResponse> ExecuteAsync(IHttpClient httpClient, CancellationToken cancellationToken);
    }
}
