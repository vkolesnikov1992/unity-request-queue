using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class BreedItem : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _label;

        [SerializeField]
        private LoadingSpin _loadingSpin;

        private Action<BreedItem> _clicked;
        public DogBreedListItem Breed { get; private set; }

        public void Initialize(int index, DogBreedListItem breed, Action<BreedItem> clicked)
        {
            Breed = breed;
            _clicked = clicked;
            _label.text = $"{index} - {breed.Name}";
            SetLoading(false);
            _button.onClick.RemoveListener(OnClicked);
            _button.onClick.AddListener(OnClicked);
        }

        public void SetLoading(bool isLoading)
        {
            _loadingSpin.gameObject.SetActive(isLoading);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _clicked.Invoke(this);
        }
    }
}
