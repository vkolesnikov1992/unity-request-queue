using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Bootstrap;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class LoadingScreenCompleteStartupStep : IAppStartupStep
    {
        private readonly LoadingScreenService _loadingScreen;

        [Preserve]
        public LoadingScreenCompleteStartupStep(LoadingScreenService loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public AppStartupStage Stage => AppStartupStage.Started;

        public int Order => int.MaxValue;

        public string Name => nameof(LoadingScreenCompleteStartupStep);

        public UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            _loadingScreen.HideStartup();
            return UniTask.CompletedTask;
        }
    }
}
