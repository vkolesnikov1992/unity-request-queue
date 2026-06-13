using TMPro;
using UnityEngine;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private string _defaultMessage;

        [SerializeField]
        private TMP_Text _message;

        public void SetVisible(bool visible)
        {
            CacheComponents();
            gameObject.SetActive(visible);

            if (_canvasGroup == null)
            {
                return;
            }

            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        public void SetMessage(string message)
        {
            CacheComponents();

            if (_message == null)
            {
                return;
            }

            _message.text = string.IsNullOrWhiteSpace(message) ? _defaultMessage : message;
        }

        private void CacheComponents()
        {
            if (_canvasGroup == null)
            {
                TryGetComponent(out _canvasGroup);
            }

            if (_message == null)
            {
                _message = GetComponentInChildren<TMP_Text>(true);
            }

            if (_message != null && _defaultMessage == null)
            {
                _defaultMessage = _message.text;
            }
        }
    }
}
