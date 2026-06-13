using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Audio
{
    public sealed class AudioService : IAudioService, IDisposable
    {
        private const string AudioObjectName = "SoundSource";

        private readonly IAssetProvider _assetProvider;
        private readonly AudioSource _soundEffectsSource;
        private readonly Dictionary<object, AudioClip> _loadedClips = new();

        private bool _disposed;

        [Preserve]
        public AudioService(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            _soundEffectsSource = CreateSoundEffectsSource();
        }

        public async UniTask PlaySoundEffectAsync(object key, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var clip = await LoadSoundEffectAsync(key, cancellationToken);
            
            ThrowIfDisposed();

            _soundEffectsSource.PlayOneShot(clip);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var clip in _loadedClips.Values)
            {
                _assetProvider.Release(clip);
            }

            _loadedClips.Clear();

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(_soundEffectsSource.gameObject);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(_soundEffectsSource.gameObject);
            }
        }

        private static AudioSource CreateSoundEffectsSource()
        {
            var audioObject = new GameObject(AudioObjectName);
            UnityEngine.Object.DontDestroyOnLoad(audioObject);

            var audioSource = audioObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            return audioSource;
        }

        private async UniTask<AudioClip> LoadSoundEffectAsync(
            object key,
            CancellationToken cancellationToken)
        {
            if (_loadedClips.TryGetValue(key, out var cachedClip))
            {
                return cachedClip;
            }

            var clip = await _assetProvider.LoadAsync<AudioClip>(key, cancellationToken);

            if (!clip)
            {
                throw new InvalidOperationException($"Loaded sound effect is null. Key: {key}");
            }

            if (_disposed)
            {
                _assetProvider.Release(clip);
                throw new ObjectDisposedException(nameof(AudioService));
            }

            if (_loadedClips.TryGetValue(key, out cachedClip))
            {
                _assetProvider.Release(clip);
                return cachedClip;
            }

            _loadedClips.Add(key, clip);

            return clip;
        }

        private void ThrowIfDisposed()
        {
            if (!_disposed)
            {
                return;
            }

            throw new ObjectDisposedException(nameof(AudioService));
        }
    }
}
