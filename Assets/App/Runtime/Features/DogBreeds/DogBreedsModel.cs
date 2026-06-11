using System.Collections.Generic;

namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class DogBreedsModel
    {
        public bool IsLoadingBreeds { get; set; }

        public bool IsLoadingBreedDetails { get; set; }

        public List<DogBreedListItem> Breeds { get; } = new();

        public DogBreedListItem SelectedBreed { get; set; }

        public string Error { get; set; }
    }

    public sealed class DogBreedListItem
    {
        public DogBreedListItem(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }

        public string Name { get; }
    }
}
