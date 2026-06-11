using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Components
{
    public sealed class ResourceCounterView : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Text _valueText;

        public Image Icon => _icon;
        public Text ValueText => _valueText;
    }
}
