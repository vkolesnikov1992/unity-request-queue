using System;
using R3;

namespace UnityRequestQueue.Runtime.User
{
    public sealed class UserResource
    {
        private readonly int? _maxValue;

        public UserResource(int initialValue, int? maxValue = null)
        {
            if (initialValue < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialValue));
            }

            if (maxValue.HasValue && maxValue.Value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            _maxValue = maxValue;

            if (_maxValue.HasValue && initialValue > _maxValue.Value)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(initialValue),
                    $"Initial resource value cannot be greater than max value '{_maxValue.Value}'.");
            }

            Amount = new ReactiveProperty<int>(initialValue);
        }

        public ReactiveProperty<int> Amount { get; }

        public void Add(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            var value = Amount.Value + amount;
            Amount.Value = _maxValue.HasValue ? Math.Min(value, _maxValue.Value) : value;
        }

        public bool TrySpend(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (Amount.Value < amount)
            {
                return false;
            }

            Amount.Value -= amount;
            return true;
        }
    }
}
