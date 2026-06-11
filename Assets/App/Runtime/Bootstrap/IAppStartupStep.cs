using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public interface IAppStartupStep
    {
        AppStartupStage Stage { get; }

        int Order { get; }

        string Name { get; }

        UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken);
    }
}
