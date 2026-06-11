using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public interface IAppEntryPoint
    {
        UniTask RunAsync(CancellationToken cancellationToken);
    }
}
