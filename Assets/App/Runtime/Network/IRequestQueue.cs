namespace UnityRequestQueue.Runtime.Network
{
    public interface IRequestQueue
    {
        int PendingCount { get; }

        bool IsRunning { get; }

        RequestHandle<TResponse> Enqueue<TResponse>(
            IRequestCommand<TResponse> command,
            RequestScope scope = null);

        void Cancel(RequestHandle handle);

        int Cancel(RequestScope scope);

        void Clear();
    }
}
