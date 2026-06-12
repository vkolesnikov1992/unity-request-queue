using UnityEngine;

namespace UnityRequestQueue.Runtime.User
{
    public sealed class UserResourcesPanel : MonoBehaviour
    {
        [SerializeField]
        private UserResourceComponent _currency;
        [SerializeField]
        private UserResourceComponent _energy;

        public void Initialize(UserResources resources)
        {
            _currency.Initialize(resources.Currency);
            _energy.Initialize(resources.Energy);
        }
    }
}
