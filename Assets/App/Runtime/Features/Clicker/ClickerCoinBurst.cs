using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.Pooling;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerCoinBurst : MonoBehaviour
    {
        [SerializeField]
        private Image _sourceImage;
        [SerializeField]
        private RectTransform _particleRoot;
        [SerializeField]
        private int _preloadCount = 30;
        [SerializeField]
        private int _coinsPerBurst = 10;
        [SerializeField]
        private Vector2 _coinSize = new(34f, 34f);
        [SerializeField]
        private float _spawnRadius = 10f;
        [SerializeField]
        private Vector2 _peakHeightRange = new(110f, 170f);
        [SerializeField]
        private Vector2 _peakHorizontalRange = new(25f, 90f);
        [SerializeField]
        private Vector2 _endHorizontalRange = new(70f, 160f);
        [SerializeField]
        private Vector2 _fallDistanceRange = new(90f, 160f);
        [SerializeField]
        private Vector2 _durationRange = new(0.55f, 0.85f);
        [SerializeField]
        private Vector2 _rotationRange = new(180f, 540f);
        [SerializeField]
        private float _startScale = 1f;
        [SerializeField]
        private float _endScale = 0.65f;

        private IComponentPool<ClickerCoinParticle> _particlePool;

        public RectTransform ParticleRoot => _particleRoot;

        public int PreloadCount => _preloadCount;

        public void Initialize(IComponentPool<ClickerCoinParticle> particlePool)
        {
            _particlePool = particlePool;
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            if (_particlePool == null)
            {
                return;
            }

            try
            {
                for (var i = 0; i < _coinsPerBurst; i++)
                {
                    var particle = await _particlePool.RentAsync(cancellationToken);
                    particle.Play(
                        _sourceImage.rectTransform.position,
                        _sourceImage.sprite,
                        _sourceImage.color,
                        CreateMotion(i),
                        _particlePool.Return);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
            }
        }

        private ClickerCoinParticleMotion CreateMotion(int index)
        {
            var side = index % 2 == 0 ? -1f : 1f;
            var startOffset = UnityEngine.Random.insideUnitCircle * _spawnRadius;
            var peakOffset = new Vector2(
                side * UnityEngine.Random.Range(_peakHorizontalRange.x, _peakHorizontalRange.y),
                UnityEngine.Random.Range(_peakHeightRange.x, _peakHeightRange.y));
            var endOffset = new Vector2(
                side * UnityEngine.Random.Range(_endHorizontalRange.x, _endHorizontalRange.y),
                -UnityEngine.Random.Range(_fallDistanceRange.x, _fallDistanceRange.y));
            var rotation = side * UnityEngine.Random.Range(_rotationRange.x, _rotationRange.y);

            return new ClickerCoinParticleMotion(
                startOffset,
                peakOffset,
                endOffset,
                _coinSize,
                _startScale,
                _endScale,
                UnityEngine.Random.Range(_durationRange.x, _durationRange.y),
                rotation);
        }
    }
}
