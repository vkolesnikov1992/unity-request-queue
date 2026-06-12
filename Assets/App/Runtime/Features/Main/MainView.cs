using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Features.Main
{
    [PresenterBinding(presenter: typeof(MainPresenter))]
    public sealed class MainView : ViewBase
    {
        [SerializeField]
        private MainTabsPanel _tabs;
        
        public MainTabsPanel Tabs => _tabs;
    }
}
