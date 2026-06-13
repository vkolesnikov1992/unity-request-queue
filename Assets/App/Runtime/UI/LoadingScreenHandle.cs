using System;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class LoadingScreenHandle : IDisposable
    {
        private readonly LoadingScreenService _service;
        private bool _disposed;

        internal LoadingScreenHandle(LoadingScreenService service, int id)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            Id = id;
        }

        internal int Id { get; }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _service.Release(this);
        }
    }
}
