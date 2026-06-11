using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Features.Main
{
    [PresenterBinding(presenter: typeof(MainPresenter))]
    public sealed class MainView : ViewBase
    {
        [SerializeField]
        private ResourcePanelView _resourcePanel;
        [SerializeField]
        private MainTabsView _tabs;

        public ResourcePanelView ResourcePanel => _resourcePanel;
        public MainTabsView Tabs => _tabs;
    }
}
