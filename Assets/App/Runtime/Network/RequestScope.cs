using System;
using System.Threading;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Network
{
    public sealed class RequestScope : IDisposable
    {
        private readonly CancellationTokenSource _cancellation = new();

        [Preserve]
        public RequestScope(string name = null)
        {
            Id = Guid.NewGuid();
            Name = string.IsNullOrWhiteSpace(name) ? Id.ToString("N") : name;
        }

        public Guid Id { get; }

        public string Name { get; }

        public CancellationToken Token => _cancellation.Token;

        public bool IsCancellationRequested => _cancellation.IsCancellationRequested;

        public void Cancel()
        {
            if (!_cancellation.IsCancellationRequested)
            {
                _cancellation.Cancel();
            }
        }

        public void Dispose()
        {
            Cancel();
            _cancellation.Dispose();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
