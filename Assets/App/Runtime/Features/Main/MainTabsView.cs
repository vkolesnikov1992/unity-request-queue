using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.Features.Main
{
    [Serializable]
    public sealed class MainTabsView
    {
        [SerializeField]
        private Transform _tabRoot;
        [SerializeField]
        private Button _clickerTabButton;
        [SerializeField]
        private Button _weatherTabButton;
        [SerializeField]
        private Button _dogBreedsTabButton;

        public Transform TabRoot => _tabRoot;
        public Button ClickerTabButton => _clickerTabButton;
        public Button WeatherTabButton => _weatherTabButton;
        public Button DogBreedsTabButton => _dogBreedsTabButton;
    }
}
