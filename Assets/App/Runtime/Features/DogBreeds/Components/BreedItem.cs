using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class BreedItem : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        [SerializeField]
        private TextMeshProUGUI _label;

        private Action<DogBreedListItem> _clicked;
        public DogBreedListItem Breed { get; private set; }

        public void Initialize(int index, DogBreedListItem breed, Action<DogBreedListItem> clicked)
        {
            Breed = breed;
            _clicked = clicked;
            _label.text = $"{index} - {breed.Name}";
            _button.onClick.RemoveListener(OnClicked);
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _clicked?.Invoke(Breed);
        }
    }
}
