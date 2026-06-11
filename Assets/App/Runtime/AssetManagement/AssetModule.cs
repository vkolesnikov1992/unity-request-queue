using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.AssetManagement
{
    public sealed class AssetModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            builder.Container
                   .Bind<IAssetProvider>()
                   .To<AddressablesAssetProvider>()
                   .AsSingle();

            builder.Container
                   .Bind<IAppStartupStep>()
                   .To<AssetProviderStartupStep>()
                   .AsSingle();
        }
    }
}
