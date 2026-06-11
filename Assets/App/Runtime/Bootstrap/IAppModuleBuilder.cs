using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public interface IAppModuleBuilder
    {
        DiContainer Container { get; }

        void RegisterModule(IAppModule module);
    }
}
