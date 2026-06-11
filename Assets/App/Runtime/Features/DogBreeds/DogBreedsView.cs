using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    [PresenterBinding(presenter: typeof(DogBreedsPresenter), model: typeof(DogBreedsModel))]
    public sealed class DogBreedsView : ViewBase
    {
        [SerializeField]
        private Transform _listRoot;
        [SerializeField]
        private Button _breedItemTemplate;
        [SerializeField]
        private GameObject _breedsLoader;
        [SerializeField]
        private GameObject _breedDetailsLoader;
        [SerializeField]
        private Text _errorText;
        [SerializeField]
        private BreedDetailsPopupView _detailsPopup;

        public Transform ListRoot => _listRoot;
        public Button BreedItemTemplate => _breedItemTemplate;
        public GameObject BreedsLoader => _breedsLoader;
        public GameObject BreedDetailsLoader => _breedDetailsLoader;
        public Text ErrorText => _errorText;
        public BreedDetailsPopupView DetailsPopup => _detailsPopup;
    }
}
