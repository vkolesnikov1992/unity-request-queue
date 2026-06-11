using System;
using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    internal sealed class AppModuleBuilder : IAppModuleBuilder
    {
        public AppModuleBuilder(DiContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public DiContainer Container { get; }

        public void RegisterModule(IAppModule module)
        {
            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            module.Register(this);
        }
    }
}
