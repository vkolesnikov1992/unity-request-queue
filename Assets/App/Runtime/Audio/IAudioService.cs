using Cysharp.Threading.Tasks;
using System.Threading;

namespace UnityRequestQueue.Runtime.Audio
{
    public interface IAudioService
    {
        UniTask PlaySoundEffectAsync(object key, CancellationToken cancellationToken);
    }
}
