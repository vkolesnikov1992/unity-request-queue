using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class UIModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            builder.Container
                   .Bind<UICanvasRoot>()
                   .FromMethod(_ => UICanvasRoot.FindInScene())
                   .AsSingle();

            builder.Container
                   .Bind<LoadingScreenService>()
                   .AsSingle();

            builder.Container
                   .Bind<ILoadingScreenService>()
                   .To<LoadingScreenService>()
                   .FromResolve();

            builder.Container
                   .Bind<IAppStartupStep>()
                   .To<LoadingScreenStartupStep>()
                   .AsSingle();

            builder.Container
                   .Bind<IAppStartupStep>()
                   .To<LoadingScreenCompleteStartupStep>()
                   .AsSingle();
        }
    }
}
