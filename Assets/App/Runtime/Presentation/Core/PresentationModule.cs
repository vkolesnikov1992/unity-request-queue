using UnityRequestQueue.Runtime.Bootstrap;
using UnityRequestQueue.Runtime.Factories;

namespace UnityRequestQueue.Runtime.Presentation
{
    public sealed class PresentationModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            builder.Container
                   .Bind<IAsyncFactory<PresenterRequest, PresenterHandle>>()
                   .To<PresenterFactory>()
                   .AsSingle();
        }
    }
}
