namespace UnityRequestQueue.Runtime.Network
{
    public interface IRequestQueue
    {
        event System.Action Changed;

        int PendingCount { get; }

        bool IsRunning { get; }

        int ActiveCount { get; }

        int LoadingScreenActiveCount { get; }

        RequestHandle<TResponse> Enqueue<TResponse>(
            IRequestCommand<TResponse> command,
            RequestScope scope = null,
            bool showLoadingScreen = true);

        void Cancel(RequestHandle handle);

        int Cancel(RequestScope scope);

        void Clear();
    }
}
