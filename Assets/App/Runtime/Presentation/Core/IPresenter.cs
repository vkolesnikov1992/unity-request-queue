using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace UnityRequestQueue.Runtime.Presentation
{
    public interface IPresenter : IDisposable
    {
        UniTask EnterAsync(CancellationToken cancellationToken);

        UniTask ExitAsync(CancellationToken cancellationToken);
    }
}
