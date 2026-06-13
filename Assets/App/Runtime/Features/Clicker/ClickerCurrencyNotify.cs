using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.Pooling;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public readonly struct ClickerCurrencyNotifyMotion
    {
        public ClickerCurrencyNotifyMotion(
            Vector2 startOffset,
            Vector2 endOffset,
            float startScale,
            float endScale,
            float durationSeconds,
            float fadeDurationPercent)
        {
            StartOffset = startOffset;
            EndOffset = endOffset;
            StartScale = startScale;
            EndScale = endScale;
            DurationSeconds = durationSeconds;
            FadeDurationPercent = fadeDurationPercent;
        }

        public Vector2 StartOffset { get; }
        public Vector2 EndOffset { get; }
        public float StartScale { get; }
        public float EndScale { get; }
        public float DurationSeconds { get; }
        public float FadeDurationPercent { get; }
    }

    [RequireComponent(typeof(CanvasGroup))]
    public sealed class ClickerCurrencyNotify : MonoBehaviour, IPoolableComponent
    {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private Image _iconImage;
        [SerializeField]
        private TextMeshProUGUI _amountText;
        [SerializeField]
        private CanvasGroup _canvasGroup;

        private Sequence _sequence;
        private Action<ClickerCurrencyNotify> _completed;

        public void Play(
            Vector3 worldPosition,
            Sprite icon,
            Color iconColor,
            string amountText,
            ClickerCurrencyNotifyMotion motion,
            Action<ClickerCurrencyNotify> completed)
        {
            _completed = completed;
            Prepare(worldPosition, icon, iconColor, amountText, motion);

            var startPosition = _rectTransform.anchoredPosition;
            var endPosition = startPosition + motion.EndOffset;
            var fadeDurationPercent = Mathf.Clamp01(motion.FadeDurationPercent);
            var durationSeconds = Mathf.Max(0.01f, motion.DurationSeconds);

            _sequence = DOTween.Sequence()
                .Join(_rectTransform.DOAnchorPos(endPosition, durationSeconds)
                    .SetEase(Ease.OutCubic))
                .Join(_rectTransform.DOScale(motion.EndScale, durationSeconds)
                    .SetEase(Ease.OutQuad))
                .Join(_canvasGroup.DOFade(0f, durationSeconds * fadeDurationPercent)
                    .SetDelay(durationSeconds * (1f - fadeDurationPercent)))
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

            _canvasGroup.alpha = 1f;
        }

        private void Prepare(
            Vector3 worldPosition,
            Sprite icon,
            Color iconColor,
            string amountText,
            ClickerCurrencyNotifyMotion motion)
        {
            KillAnimation();

            _rectTransform.anchoredPosition = GetAnchoredPosition(worldPosition) + motion.StartOffset;
            _rectTransform.localRotation = Quaternion.identity;
            _rectTransform.localScale = Vector3.one * motion.StartScale;

            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;

            _iconImage.sprite = icon;
            _iconImage.color = iconColor;
            _iconImage.preserveAspect = true;
            _iconImage.raycastTarget = false;

            _amountText.text = amountText;
            _amountText.raycastTarget = false;
            _amountText.ForceMeshUpdate();
        }

        private void ReturnToPool()
        {
            var completed = _completed;
            _completed = null;
            completed.Invoke(this);
        }

        private Vector2 GetAnchoredPosition(Vector3 worldPosition)
        {
            var parent = (RectTransform)_rectTransform.parent;
            var canvas = parent.GetComponentInParent<Canvas>();
            var camera = canvas.renderMode != RenderMode.ScreenSpaceOverlay
                ? canvas.worldCamera
                : null;
            var screenPosition = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent,
                screenPosition,
                camera,
                out var localPosition);

            return localPosition;
        }

        private void KillAnimation()
        {
            _sequence?.Kill();
            _sequence = null;
        }
    }
}
