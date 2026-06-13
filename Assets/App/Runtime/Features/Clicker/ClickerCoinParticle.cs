using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.Pooling;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public readonly struct ClickerCoinParticleMotion
    {
        public ClickerCoinParticleMotion(
            Vector2 startOffset,
            Vector2 peakOffset,
            Vector2 endOffset,
            Vector2 size,
            float startScale,
            float endScale,
            float durationSeconds,
            float rotationDegrees)
        {
            StartOffset = startOffset;
            PeakOffset = peakOffset;
            EndOffset = endOffset;
            Size = size;
            StartScale = startScale;
            EndScale = endScale;
            DurationSeconds = durationSeconds;
            RotationDegrees = rotationDegrees;
        }

        public Vector2 StartOffset { get; }
        public Vector2 PeakOffset { get; }
        public Vector2 EndOffset { get; }
        public Vector2 Size { get; }
        public float StartScale { get; }
        public float EndScale { get; }
        public float DurationSeconds { get; }
        public float RotationDegrees { get; }
    }

    [RequireComponent(typeof(Image))]
    public sealed class ClickerCoinParticle : MonoBehaviour, IPoolableComponent
    {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private Image _image;

        private Sequence _sequence;
        private Action<ClickerCoinParticle> _completed;

        public void Play(
            Vector3 worldPosition,
            Sprite sprite,
            Color color,
            ClickerCoinParticleMotion motion,
            Action<ClickerCoinParticle> completed)
        {
            _completed = completed;
            Prepare(worldPosition, sprite, color, motion);

            var startPosition = _rectTransform.localPosition;
            var path = new Vector3[]
            {
                startPosition,
                startPosition + (Vector3)motion.PeakOffset,
                startPosition + (Vector3)motion.EndOffset
            };

            _sequence = DOTween.Sequence()
                .Join(_rectTransform.DOLocalPath(path, motion.DurationSeconds, PathType.CatmullRom)
                    .SetEase(Ease.InQuad))
                .Join(_rectTransform.DORotate(Vector3.forward * motion.RotationDegrees, motion.DurationSeconds, RotateMode.FastBeyond360)
                    .SetEase(Ease.OutQuad))
                .Join(_rectTransform.DOScale(motion.EndScale, motion.DurationSeconds)
                    .SetEase(Ease.OutQuad))
                .Join(_image.DOFade(0f, motion.DurationSeconds * 0.35f)
                    .SetDelay(motion.DurationSeconds * 0.65f))
                .OnComplete(ReturnToPool);
        }

        public void OnRentFromPool()
        {
            gameObject.SetActive(true);
            KillAnimation();
        }

        public void OnReturnToPool()
        {
            _completed = null;
            KillAnimation();
        }

        private void Prepare(
            Vector3 worldPosition,
            Sprite sprite,
            Color color,
            ClickerCoinParticleMotion motion)
        {
            KillAnimation();

            _rectTransform.position = worldPosition;
            _rectTransform.localPosition += (Vector3)motion.StartOffset;
            _rectTransform.localRotation = Quaternion.identity;
            _rectTransform.localScale = Vector3.one * motion.StartScale;
            _rectTransform.sizeDelta = motion.Size;

            _image.sprite = sprite;
            _image.color = color;
            _image.preserveAspect = true;
            _image.raycastTarget = false;
        }

        private void ReturnToPool()
        {
            var completed = _completed;
            _completed = null;
            completed.Invoke(this);
        }

        private void KillAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
