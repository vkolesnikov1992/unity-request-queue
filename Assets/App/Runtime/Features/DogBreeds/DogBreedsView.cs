using UnityEngine;
using UnityRequestQueue.Runtime.Presentation;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    [PresenterBinding(presenter: typeof(DogBreedsPresenter), model: typeof(DogBreedsModel))]
    public sealed class DogBreedsView : ViewBase
    {
        [SerializeField]
        private Transform _breedsItemContainer;
        
        [SerializeField]
        private BreedDetailsPopup _detailsPopupPrefab;

        private BreedDetailsPopup _detailsPopup;

        public Transform BreedsItemContainer => _breedsItemContainer;

        public void CreatePopup(Transform root)
        {
            if (_detailsPopup)
            {
                return;
            }

            _detailsPopup = Instantiate(_detailsPopupPrefab, root);
            _detailsPopup.Hide();
        }

        public void ShowPopup(string title, string description)
        {
            _detailsPopup.Show(title, description);
        }

        public void HidePopup()
        {
            _detailsPopup.Hide();
        }

        private void OnDestroy()
        {
            if (!_detailsPopup)
            {
                return;
            }

            Destroy(_detailsPopup.gameObject);
            _detailsPopup = null;
        }
    }
}
