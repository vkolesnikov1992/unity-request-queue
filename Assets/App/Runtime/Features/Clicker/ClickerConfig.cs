using UnityEngine;

namespace UnityRequestQueue.Runtime.Features.Clicker
{
    [CreateAssetMenu(menuName = "Unity Request Queue/Clicker Config", fileName = "ClickerConfig")]
    public sealed class ClickerConfig : ScriptableObject
    {
        [SerializeField]
        private int _currencyPerClick = 1;
        [SerializeField]
        private int _energyCostPerClick = 1;
        [SerializeField]
        private int _maxEnergy = 1000;
        [SerializeField]
        private int _initialEnergy = 1000;
        [SerializeField]
        private int _energyRestoreAmount = 10;
        [SerializeField]
        private float _energyRestoreIntervalSeconds = 10f;
        [SerializeField]
        private float _autoCollectIntervalSeconds = 3f;
        [SerializeField]
        private bool _autoCollectWhenInactive;
        [SerializeField]
        private string _clickSoundKey;
        [SerializeField]
        private float _buttonPunchScale = 0.08f;
        [SerializeField]
        private float _buttonPunchDurationSeconds = 0.18f;
        [SerializeField]
        private int _buttonPunchVibrato = 8;
        [SerializeField]
        private float _buttonPunchElasticity = 0.7f;

        public int CurrencyPerClick => _currencyPerClick;
        public int EnergyCostPerClick => _energyCostPerClick;
        public int MaxEnergy => _maxEnergy;
        public int InitialEnergy => _initialEnergy;
        public int EnergyRestoreAmount => _energyRestoreAmount;
        public float EnergyRestoreIntervalSeconds => _energyRestoreIntervalSeconds;
        public float AutoCollectIntervalSeconds => _autoCollectIntervalSeconds;
        public bool AutoCollectWhenInactive => _autoCollectWhenInactive;
        public string ClickSoundKey => _clickSoundKey;
        public float ButtonPunchScale => _buttonPunchScale;
        public float ButtonPunchDurationSeconds => _buttonPunchDurationSeconds;
        public int ButtonPunchVibrato => _buttonPunchVibrato;
        public float ButtonPunchElasticity => _buttonPunchElasticity;
    }
}
