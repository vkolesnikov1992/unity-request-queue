using UnityRequestQueue.Runtime.Bootstrap;

namespace UnityRequestQueue.Runtime.Audio
{
    public sealed class AudioModule : IAppModule
    {
        public void Register(IAppModuleBuilder builder)
        {
            builder.Container
                   .Bind<IAudioService>()
                   .To<AudioService>()
                   .AsSingle();
        }
    }
}
