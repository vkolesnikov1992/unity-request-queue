namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerModel
    {
        public ClickerModel()
        {
        }

        public ClickerModel(int currency, int energy)
        {
            Currency = currency;
            Energy = energy;
        }

        public int Currency { get; set; }

        public int Energy { get; set; }
    }
}
