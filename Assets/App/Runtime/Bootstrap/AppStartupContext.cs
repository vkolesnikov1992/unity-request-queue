using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public sealed class AppStartupContext
    {
        internal AppStartupContext(DiContainer container)
        {
            Container = container;
        }

        public DiContainer Container { get; }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
