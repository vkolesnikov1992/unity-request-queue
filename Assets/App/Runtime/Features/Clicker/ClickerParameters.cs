namespace UnityRequestQueue.Runtime.Features.Clicker
{
    public sealed class ClickerParameters
    {
        public ClickerParameters(ClickerConfig config, ClickerModel initialModel = null)
        {
            Config = config;
            InitialModel = initialModel;
        }

        public ClickerConfig Config { get; }

        public ClickerModel InitialModel { get; }
    }
}
