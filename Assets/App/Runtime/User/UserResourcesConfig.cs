using UnityEngine;

namespace UnityRequestQueue.Runtime.User
{
    [CreateAssetMenu(menuName = "Unity Request Queue/User Resources Config", fileName = "UserResourcesConfig")]
    public sealed class UserResourcesConfig : ScriptableObject
    {
        [SerializeField]
        private int _initialCurrency;
        [SerializeField]
        private int _initialEnergy = 1000;
        [SerializeField]
        private int _maxEnergy = 1000;

        public int InitialCurrency => _initialCurrency;
        public int InitialEnergy => _initialEnergy;
        public int MaxEnergy => _maxEnergy;
    }
}
