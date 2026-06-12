using R3;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.Core
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class TabButtonComponent : MonoBehaviour
    {
        [SerializeField]
        private TabId _id;
        [SerializeField]
        private Button _button;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Color _primaryColor = new Color(0.1f, 0.35f, 0.9f, 1f);
        [SerializeField]
        private Color _secondaryColor = Color.white;

        public TabId Id => _id;

        public Observable<Unit> Clicked => _button.OnClickAsObservable();

        public void SetSelected(bool selected)
        {
            _background.color = selected ? _secondaryColor : _primaryColor;
            _icon.color = selected ? _primaryColor : _secondaryColor;
        }
    }
}
