namespace UnityRequestQueue.Runtime.Features.DogBreeds
{
    public sealed class DogBreedsModel
    {
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

    public sealed class DogBreedDetails
    {
        public DogBreedDetails(string id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; }

        public string Name { get; }

        public string Description { get; }
    }
}
