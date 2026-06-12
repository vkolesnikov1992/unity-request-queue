using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityRequestQueue.Runtime.User
{
    public sealed class UserResourceComponent : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private TextMeshProUGUI _amountText;

        private UserResource _resource;
        private IDisposable _subscription;

        public void Initialize(UserResource resource)
        {
            _resource = resource;

            Subscribe();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            DisposeSubscription();
        }

        private void OnDestroy()
        {
            DisposeSubscription();
        }

        private void Subscribe()
        {
            DisposeSubscription();

            if (_resource == null)
            {
                return;
            }

            SetAmount(_resource.Amount.Value);
            _subscription = _resource.Amount.Subscribe(SetAmount);
        }

        private void SetAmount(int amount)
        {
            _amountText.text = amount.ToString();
        }

        private void DisposeSubscription()
        {
            _subscription?.Dispose();
            _subscription = null;
        }
    }
}
