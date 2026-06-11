namespace UnityRequestQueue.Runtime.Bootstrap
{
    public sealed class CoreModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            var container = builder.Container;

            container.Bind<Application>().AsSingle();
            container.Bind<IAppEntryPoint>().To<Application>().FromResolve();
            container.Bind<AppStartupContext>().FromMethod(_ => new AppStartupContext(container)).AsTransient();
            container.Bind<AppStartupPipeline>()
                     .FromMethod(_ => new AppStartupPipeline(container.ResolveAll<IAppStartupStep>()))
                     .AsTransient();
        }
    }
}
