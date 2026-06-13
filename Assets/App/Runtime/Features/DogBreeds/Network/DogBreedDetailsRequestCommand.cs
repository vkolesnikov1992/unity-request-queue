using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.DogBreeds.Network
{
    internal sealed class DogBreedDetailsRequestCommand : IRequestCommand<DogBreedDetails>
    {
        private readonly string _baseUrl;
        private readonly string _breedId;

        public DogBreedDetailsRequestCommand(string baseUrl, string breedId)
        {
            _baseUrl = baseUrl;
            _breedId = breedId;
        }

        public string Name => $"Dog breed {_baseUrl.TrimEnd('/')}/{_breedId}";

        public async UniTask<DogBreedDetails> ExecuteAsync(
            IHttpClient httpClient,
            CancellationToken cancellationToken)
        {
            var response = await httpClient.SendAsync(
                HttpRequest.Get($"{_baseUrl.TrimEnd('/')}/{_breedId}"),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Dog breed request failed with status code {response.StatusCode}.");
            }

            var detailsResponse = JsonUtility.FromJson<DogBreedDetailsResponse>(response.Text);
            var breed = detailsResponse.data;

            return new DogBreedDetails(
                breed.id,
                breed.attributes.name,
                breed.attributes.description);
        }
    }
}
