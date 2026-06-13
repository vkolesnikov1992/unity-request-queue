using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Features.DogBreeds.Network;
using UnityRequestQueue.Runtime.Network;
using UnityRequestQueue.Runtime.UI;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class DogBreedsPresenter : NetworkTabPresenterBase<DogBreedsView, DogBreedsParameters, DogBreedsModel>
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IAssetProvider _assetProvider;
        private readonly ILoadingScreenService _loadingScreen;
        private readonly UICanvasRoot _canvasRoot;
        private readonly List<GameObject> _breedItemInstances = new();

        private RequestHandle<DogBreedDetails> _breedDetailsHandle;

        [Preserve]
        public DogBreedsPresenter(
            IRequestQueue requestQueue,
            IAssetProvider assetProvider,
            ILoadingScreenService loadingScreen,
            UICanvasRoot canvasRoot)
            : base(requestQueue)
        {
            _requestQueue = requestQueue;
            _assetProvider = assetProvider;
            _loadingScreen = loadingScreen;
            _canvasRoot = canvasRoot;
        }

        public override TabId TabId => TabId.DogBreeds;

        protected override DogBreedsParameters DefaultParameters => DogBreedsParameters.Default;

        protected override void OnViewBuilt()
        {
            View.CreatePopup(_canvasRoot.Transform);
        }

        protected override UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            LoadBreedsAsync(cancellationToken).Forget(Debug.LogException);
            return UniTask.CompletedTask;
        }

        protected override UniTask OnNetworkTabExitAsync(CancellationToken cancellationToken)
        {
            CancelBreedDetailsRequest();
            View.HidePopup();
            return UniTask.CompletedTask;
        }

        protected override void OnDispose()
        {
            CancelBreedDetailsRequest();
            ReleaseBreedItems();
            base.OnDispose();
        }

        private async UniTask LoadBreedsAsync(CancellationToken cancellationToken)
        {
            Model.Error = null;
            View.HidePopup();

            if (HasCachedBreeds())
            {
                await RestoreCachedBreedsAsync(cancellationToken);
                return;
            }

            var handle = _requestQueue.Enqueue(
                new DogBreedsRequestCommand(Parameters.BreedsUrl),
                RequestScope);
            using var loading = _loadingScreen.Show("Загрузка...");

            try
            {
                var breeds = await handle.Task.AttachExternalCancellation(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Model.Breeds.Clear();
                Model.Breeds.AddRange(breeds);
                Model.BreedsUrl = Parameters.BreedsUrl;

                ReleaseBreedItems();

                for (var i = 0; i < Model.Breeds.Count; i++)
                {
                    await CreateBreedItemAsync(i + 1, Model.Breeds[i], cancellationToken);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || handle.Status == RequestStatus.Canceled)
            {
            }
            catch (Exception exception)
            {
                Model.Error = exception.Message;
                Debug.LogException(exception);
            }
        }

        private bool HasCachedBreeds()
        {
            return Model.Breeds.Count > 0 &&
                   string.Equals(Model.BreedsUrl, Parameters.BreedsUrl, StringComparison.Ordinal);
        }

        private async UniTask RestoreCachedBreedsAsync(CancellationToken cancellationToken)
        {
            if (_breedItemInstances.Count == Model.Breeds.Count)
            {
                return;
            }

            ReleaseBreedItems();

            for (var i = 0; i < Model.Breeds.Count; i++)
            {
                await CreateBreedItemAsync(i + 1, Model.Breeds[i], cancellationToken);
            }
        }

        private async UniTask CreateBreedItemAsync(
            int index,
            DogBreedListItem breed,
            CancellationToken cancellationToken)
        {
            var instance = await _assetProvider.InstantiateAsync(
                FeatureAssetKeys.BreedItem,
                View.BreedsItemContainer,
                cancellationToken);

            var item = instance.GetComponent<BreedItem>();
            item.Initialize(index, breed, OnBreedClicked);

            _breedItemInstances.Add(instance);
        }

        private void OnBreedClicked(DogBreedListItem breed)
        {
            CancelBreedDetailsRequest();
            Model.SelectedBreed = breed;
            LoadBreedDetailsAsync(breed, ActivationToken).Forget(Debug.LogException);
        }

        private async UniTask LoadBreedDetailsAsync(
            DogBreedListItem breed,
            CancellationToken cancellationToken)
        {
            Model.Error = null;
            View.HidePopup();

            var handle = _requestQueue.Enqueue(
                new DogBreedDetailsRequestCommand(Parameters.BreedDetailsBaseUrl, breed.Id),
                RequestScope);

            _breedDetailsHandle = handle;
            using var loading = _loadingScreen.Show("Загрузка данных...");

            try
            {
                var details = await handle.Task.AttachExternalCancellation(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                View.ShowPopup(details.Name, details.Description);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested || handle.Status == RequestStatus.Canceled)
            {
            }
            catch (Exception exception)
            {
                Model.Error = exception.Message;
                Debug.LogException(exception);
            }
            finally
            {
                if (_breedDetailsHandle == handle)
                {
                    _breedDetailsHandle = null;
                }
            }
        }

        private void CancelBreedDetailsRequest()
        {
            if (_breedDetailsHandle != null && !_breedDetailsHandle.IsCompleted)
            {
                _breedDetailsHandle.Cancel();
            }

            _breedDetailsHandle = null;
        }

        private void ReleaseBreedItems()
        {
            foreach (var instance in _breedItemInstances)
            {
                _assetProvider.ReleaseInstance(instance);
            }

            _breedItemInstances.Clear();
        }
    }
}
