using UnityRequestQueue.Runtime.Features.Core;

namespace UnityRequestQueue.Runtime.Features.Main
{
    public sealed class MainParameters
    {
        public static MainParameters Default { get; } = new MainParameters(TabId.Clicker);

        public MainParameters(TabId initialTab)
        {
            InitialTab = initialTab;
        }

        public TabId InitialTab { get; }
    }
}
