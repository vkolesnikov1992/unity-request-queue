using TMPro;
using UnityEngine;

namespace UnityRequestQueue.Runtime.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class LoadingScreen : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private string _defaultMessage;

        [SerializeField]
        private TextMeshProUGUI _message;

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);

            _canvasGroup.alpha = visible ? 1f : 0f;
            _canvasGroup.interactable = visible;
            _canvasGroup.blocksRaycasts = visible;
        }

        public void SetMessage(string message)
        {
            _message.text = string.IsNullOrWhiteSpace(message) ? _defaultMessage : message;
        }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _defaultMessage = _message.text;
        }
    }
}
