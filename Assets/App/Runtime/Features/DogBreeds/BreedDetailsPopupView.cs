using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class BreedDetailsPopupView : MonoBehaviour
    {
        [SerializeField]
        private Text _titleText;
        [SerializeField]
        private Text _descriptionText;
        [SerializeField]
        private Button _closeButton;
        [SerializeField]
        private RectTransform _contentRoot;

        public Text TitleText => _titleText;
        public Text DescriptionText => _descriptionText;
        public Button CloseButton => _closeButton;
        public RectTransform ContentRoot => _contentRoot;
    }
}
