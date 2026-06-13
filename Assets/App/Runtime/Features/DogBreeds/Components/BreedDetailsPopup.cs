using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class BreedDetailsPopup : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _titleText;
        
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        
        [SerializeField]
        private Button _continueButton;

        [SerializeField] 
        private VerticalLayoutGroup _layoutGroup;

        private void Awake()
        {
            _continueButton.onClick.AddListener(Hide);
        }

        private void OnDestroy()
        {
            _continueButton.onClick.RemoveListener(Hide);
        }

        public void Show(string title, string description)
        {
            _titleText.text = title;
            _descriptionText.text = description;
            gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)_layoutGroup.transform);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
