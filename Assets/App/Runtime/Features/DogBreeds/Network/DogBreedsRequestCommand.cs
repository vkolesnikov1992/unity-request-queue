using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityRequestQueue.Runtime.Network;

namespace UnityRequestQueue.Runtime.Features.DogBreeds.Network
{
    internal sealed class DogBreedsRequestCommand : IRequestCommand<List<DogBreedListItem>>
    {
        private readonly string _url;

        public DogBreedsRequestCommand(string url)
        {
            _url = url;
        }

        public string Name => $"Dog breeds {_url}";

        public async UniTask<List<DogBreedListItem>> ExecuteAsync(
            IHttpClient httpClient,
            CancellationToken cancellationToken)
        {
            var response = await httpClient.SendAsync(
                HttpRequest.Get(_url),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Dog breeds request failed with status code {response.StatusCode}.");
            }

            var breedsResponse = JsonUtility.FromJson<DogBreedsResponse>(response.Text);
            if (breedsResponse?.data == null)
            {
                throw new InvalidOperationException("Dog breeds response does not contain breeds data.");
            }

            var count = Math.Min(10, breedsResponse.data.Length);
            var breeds = new List<DogBreedListItem>(count);

            for (var i = 0; i < count; i++)
            {
                var breed = breedsResponse.data[i];
                if (breed?.attributes == null)
                {
                    continue;
                }

                breeds.Add(new DogBreedListItem(breed.id, breed.attributes.name));
            }

            return breeds;
        }
    }
}
