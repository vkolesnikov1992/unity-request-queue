using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace UnityRequestQueue.Runtime.Network
{
    public abstract class RequestHandle
    {
        private readonly Action<RequestHandle> _cancel;

        internal RequestHandle(Guid id, string name, RequestScope scope, Action<RequestHandle> cancel)
        {
            Id = id;
            Name = string.IsNullOrWhiteSpace(name) ? id.ToString("N") : name;
            Scope = scope;
            _cancel = cancel ?? throw new ArgumentNullException(nameof(cancel));
            Status = RequestStatus.Pending;
        }

        public Guid Id { get; }

        public string Name { get; }

        public RequestScope Scope { get; }

        public RequestStatus Status { get; internal set; }

        public bool IsCompleted =>
            Status == RequestStatus.Succeeded ||
            Status == RequestStatus.Failed ||
            Status == RequestStatus.Canceled;

        public void Cancel()
        {
            _cancel(this);
        }

        internal abstract void SetCanceled(CancellationToken cancellationToken);
    }

    public sealed class RequestHandle<TResponse> : RequestHandle
    {
        private readonly UniTaskCompletionSource<TResponse> _completion =
            new UniTaskCompletionSource<TResponse>();

        internal RequestHandle(Guid id, string name, RequestScope scope, Action<RequestHandle> cancel)
            : base(id, name, scope, cancel)
        {
        }

        public UniTask<TResponse> Task => _completion.Task;

        internal void SetResult(TResponse result)
        {
            Status = RequestStatus.Succeeded;
            _completion.TrySetResult(result);
        }

        internal void SetException(Exception exception)
        {
            Status = RequestStatus.Failed;
            _completion.TrySetException(exception);
        }

        internal override void SetCanceled(CancellationToken cancellationToken)
        {
            Status = RequestStatus.Canceled;
            _completion.TrySetCanceled(cancellationToken);
        }
    }
}
