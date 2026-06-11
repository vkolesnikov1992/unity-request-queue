using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.Pooling
{
    public sealed class PoolingModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            builder.Container
                   .Bind<IComponentPoolFactory>()
                   .To<ComponentPoolFactory>()
                   .AsSingle();
        }
    }
}
