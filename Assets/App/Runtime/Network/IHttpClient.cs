using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Network
{
    public interface IHttpClient
    {
        UniTask<HttpResponse> SendAsync(HttpRequest request, CancellationToken cancellationToken);
    }
}
