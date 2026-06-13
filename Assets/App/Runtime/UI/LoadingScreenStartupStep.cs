using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Bootstrap;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class LoadingScreenStartupStep : IAppStartupStep
    {
        private readonly LoadingScreenService _loadingScreen;

        [Preserve]
        public LoadingScreenStartupStep(LoadingScreenService loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public AppStartupStage Stage => AppStartupStage.Services;

        public int Order => 0;

        public string Name => nameof(LoadingScreenStartupStep);

        public async UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            await _loadingScreen.InitializeAsync(cancellationToken);
            _loadingScreen.ShowStartup();
        }
    }
}
