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
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
