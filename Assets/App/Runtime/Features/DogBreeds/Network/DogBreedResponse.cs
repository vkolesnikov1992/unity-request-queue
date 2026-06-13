using System;

namespace UnityRequestQueue.Runtime.Features.DogBreeds.Network
{
    [Serializable]
    internal sealed class DogBreedsResponse
    {
        public DogBreedData[] data;
    }

    [Serializable]
    internal sealed class DogBreedDetailsResponse
    {
        public DogBreedData data;
    }

    [Serializable]
    internal sealed class DogBreedData
    {
        public string id;
        public DogBreedAttributes attributes;
    }

    [Serializable]
    internal sealed class DogBreedAttributes
    {
        public string name;
        public string description;
    }
}
