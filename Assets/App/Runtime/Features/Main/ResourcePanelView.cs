using System;
using UnityRequestQueue.Runtime.Components;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Features.Main
{
    [Serializable]
    public sealed class ResourcePanelView
    {
        [SerializeField]
        private ResourceCounterView _currency;
        [SerializeField]
        private ResourceCounterView _energy;

        public ResourceCounterView Currency => _currency;
        public ResourceCounterView Energy => _energy;
    }
}
