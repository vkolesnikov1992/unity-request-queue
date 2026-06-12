using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;
using UnityEngine.UI;
using UnityRequestQueue.Runtime.User;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    [PresenterBinding(presenter: typeof(ClickerPresenter), model: typeof(ClickerModel))]
    public sealed class ClickerView : ViewBase
    {
        [SerializeField]
        private UserResourcesPanel _resourcePanel;
        [SerializeField]
        private Button _collectButton;
        
        public UserResourcesPanel ResourcePanel => _resourcePanel;
        public Button CollectButton => _collectButton;
        
    }
}
