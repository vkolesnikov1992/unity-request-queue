using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.Network
{
    public sealed class NetworkModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            var container = builder.Container;

            container.Bind<IHttpClient>().To<UnityWebRequestHttpClient>().AsSingle();
            container.Bind<IRequestQueue>().To<RequestQueue>().AsSingle();
        }
    }
}
