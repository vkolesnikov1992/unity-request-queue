namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class DogBreedsParameters
    {
        public static DogBreedsParameters Default { get; } = new DogBreedsParameters(
            "https://dogapi.dog/api/v2/breeds?page[size]=10",
            "https://dogapi.dog/api/v2/breeds");

        public DogBreedsParameters(string breedsUrl, string breedDetailsBaseUrl)
        {
            BreedsUrl = breedsUrl;
            BreedDetailsBaseUrl = breedDetailsBaseUrl;
        }

        public string BreedsUrl { get; }

        public string BreedDetailsBaseUrl { get; }
    }
}
