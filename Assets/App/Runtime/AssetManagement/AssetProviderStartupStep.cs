using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.AssetManagement
{
    public sealed class AssetProviderStartupStep : IAppStartupStep
    {
        private readonly IAssetProvider _assetProvider;

        public AssetProviderStartupStep(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        }

        public AppStartupStage Stage => AppStartupStage.Load;

        public int Order => 0;

        public string Name => nameof(AssetProviderStartupStep);

        public UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            return _assetProvider.InitializeAsync(cancellationToken);
        }
    }
}
