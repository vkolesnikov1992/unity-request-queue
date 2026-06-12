using UnityRequestQueue.Runtime.Bootstrap;
using UnityRequestQueue.Runtime.Features.Clicker;
using UnityRequestQueue.Runtime.User;
using Zenject;

namespace UnityRequestQueue.Runtime.Sections
{
    public sealed class SectionsModule : IAppModule
    {
        private const string UserResourcesConfigPath = "Configs/UserResourcesConfig";
        private const string ClickerConfigPath = "Configs/ClickerConfig";

        public void Register(IAppModuleBuilder builder)
        {
            var container = builder.Container;

            RegisterUserResources(container);
            RegisterClickerConfig(container);
            RegisterSection<UserSection>(container);
        }

        private void RegisterUserResources(DiContainer container)
        {
            container.Bind<UserResourcesConfig>()
                     .FromScriptableObjectResource(UserResourcesConfigPath)
                     .AsSingle();
            container.Bind<UserResources>().AsSingle();
        }

        private static void RegisterClickerConfig(DiContainer container)
        {
            container.Bind<ClickerConfig>()
                     .FromScriptableObjectResource(ClickerConfigPath)
                     .AsSingle();
        }

        private static void RegisterSection<TSection>(DiContainer container)
            where TSection : class, IAppSection
        {
            container.BindInterfacesAndSelfTo<TSection>().AsSingle();
        }
    }
}
