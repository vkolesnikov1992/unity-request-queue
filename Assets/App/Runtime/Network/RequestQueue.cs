using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Network
{
    public sealed class RequestQueue : IRequestQueue
    {
        private readonly IHttpClient _httpClient;
        private readonly List<RequestItem> _pending = new();

        private RequestItem _current;
        private CancellationTokenSource _currentCancellation;
        private bool _isProcessing;

        [Preserve]
        public RequestQueue(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public event Action Changed;

        public int PendingCount => _pending.Count;

        public bool IsRunning => _current != null;

        public int ActiveCount => _pending.Count + (IsRunning ? 1 : 0);

        public int LoadingScreenActiveCount =>
            _pending.Count(item => item.Handle.ShowLoadingScreen) +
            (_current != null && _current.Handle.ShowLoadingScreen ? 1 : 0);

        public RequestHandle<TResponse> Enqueue<TResponse>(
            IRequestCommand<TResponse> command,
            RequestScope scope = null,
            bool showLoadingScreen = true)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var item = new RequestItem<TResponse>(command, scope, showLoadingScreen, Cancel);

            if (scope != null && scope.IsCancellationRequested)
            {
                Log($"Canceled before enqueue '{item.Handle.Name}' id={item.Handle.Id:N} scope={scope.Name}");
                item.Cancel(scope.Token);
                return item.TypedHandle;
            }

            _pending.Add(item);
            Log($"Enqueued '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(scope)} pending={_pending.Count}");
            NotifyChanged();
            StartProcessingIfNeeded();

            return item.TypedHandle;
        }

        public void Cancel(RequestHandle handle)
        {
            if (handle == null)
            {
                return;
            }

            if (_current != null && _current.Handle.Id == handle.Id)
            {
                Log($"Cancel running requested '{handle.Name}' id={handle.Id:N} scope={GetScopeName(handle.Scope)}");
                _currentCancellation?.Cancel();
                return;
            }

            var item = _pending.FirstOrDefault(pending => pending.Handle.Id == handle.Id);

            if (item == null)
            {
                return;
            }

            _pending.Remove(item);
            Log($"Canceled pending '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)} pending={_pending.Count}");
            item.Cancel(item.CancellationToken);
            NotifyChanged();
        }

        public int Cancel(RequestScope scope)
        {
            if (scope == null)
            {
                return 0;
            }

            scope.Cancel();

            var canceledCount = 0;
            var pending = _pending
                .Where(item => item.Scope == scope)
                .ToArray();

            foreach (var item in pending)
            {
                _pending.Remove(item);
                Log($"Canceled pending by scope '{item.Handle.Name}' id={item.Handle.Id:N} scope={scope.Name} pending={_pending.Count}");
                item.Cancel(scope.Token);
                canceledCount++;
            }

            if (pending.Length > 0)
            {
                NotifyChanged();
            }

            if (_current != null && _current.Scope == scope)
            {
                Log($"Cancel running by scope requested '{_current.Handle.Name}' id={_current.Handle.Id:N} scope={scope.Name}");
                _currentCancellation?.Cancel();
                canceledCount++;
            }

            return canceledCount;
        }

        public void Clear()
        {
            foreach (var item in _pending)
            {
                Log($"Canceled pending by clear '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)}");
                item.Cancel(item.CancellationToken);
            }

            _pending.Clear();
            NotifyChanged();

            if (_current != null)
            {
                Log($"Cancel running by clear requested '{_current.Handle.Name}' id={_current.Handle.Id:N} scope={GetScopeName(_current.Scope)}");
            }

            _currentCancellation?.Cancel();
        }

        private void StartProcessingIfNeeded()
        {
            if (_isProcessing)
            {
                return;
            }

            _isProcessing = true;
            ProcessAsync().Forget(Debug.LogException);
        }

        private async UniTask ProcessAsync()
        {
            while (true)
            {
                if (_pending.Count == 0)
                {
                    _isProcessing = false;
                    return;
                }

                var item = _pending[0];
                _pending.RemoveAt(0);

                if (item.IsCancellationRequested)
                {
                    Log($"Skipped canceled '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)}");
                    item.Cancel(item.CancellationToken);
                    NotifyChanged();
                    continue;
                }

                _current = item;
                _currentCancellation = item.CreateCancellation();
                item.SetRunning();
                Log($"Started '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)} pending={_pending.Count}");
                NotifyChanged();

                try
                {
                    await item.ExecuteAsync(_httpClient, _currentCancellation.Token);
                    Log($"Succeeded '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)}");
                }
                catch (OperationCanceledException)
                {
                    Log($"Canceled running '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)}");
                    item.Cancel(_currentCancellation.Token);
                }
                catch (Exception exception)
                {
                    Log($"Failed '{item.Handle.Name}' id={item.Handle.Id:N} scope={GetScopeName(item.Scope)} error={exception.Message}");
                    item.Fail(exception);
                }
                finally
                {
                    _currentCancellation.Dispose();
                    _currentCancellation = null;
                    _current = null;
                    NotifyChanged();
                }
            }
        }

        private static void Log(string message)
        {
            Debug.Log($"[RequestQueue] {message}");
        }

        private static string GetScopeName(RequestScope scope)
        {
            return scope == null ? "none" : scope.Name;
        }

        private void NotifyChanged()
        {
            try
            {
                Changed?.Invoke();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        #region Classes

        private abstract class RequestItem
        {
            protected RequestItem(RequestScope scope)
            {
                Scope = scope;
            }

            public abstract RequestHandle Handle { get; }

            public RequestScope Scope { get; }

            public bool IsCancellationRequested =>
                Scope != null && Scope.IsCancellationRequested;

            public CancellationToken CancellationToken =>
                Scope != null ? Scope.Token : CancellationToken.None;

            public CancellationTokenSource CreateCancellation()
            {
                return Scope == null
                    ? new CancellationTokenSource()
                    : CancellationTokenSource.CreateLinkedTokenSource(Scope.Token);
            }

            public void SetRunning()
            {
                Handle.Status = RequestStatus.Running;
            }

            public abstract UniTask ExecuteAsync(IHttpClient httpClient, CancellationToken cancellationToken);

            public abstract void Fail(Exception exception);

            public abstract void Cancel(CancellationToken cancellationToken);
        }

        private sealed class RequestItem<TResponse> : RequestItem
        {
            private readonly IRequestCommand<TResponse> _command;
            private readonly RequestHandle<TResponse> _handle;

            public RequestItem(
                IRequestCommand<TResponse> command,
                RequestScope scope,
                bool showLoadingScreen,
                Action<RequestHandle> cancel)
                : base(scope)
            {
                _command = command ?? throw new ArgumentNullException(nameof(command));
                _handle = new RequestHandle<TResponse>(
                    Guid.NewGuid(),
                    command.Name,
                    scope,
                    showLoadingScreen,
                    cancel);
            }

            public override RequestHandle Handle => _handle;

            public RequestHandle<TResponse> TypedHandle => _handle;

            public override async UniTask ExecuteAsync(
                IHttpClient httpClient,
                CancellationToken cancellationToken)
            {
                var result = await _command.ExecuteAsync(httpClient, cancellationToken);
                _handle.SetResult(result);
            }

            public override void Fail(Exception exception)
            {
                _handle.SetException(exception);
            }

            public override void Cancel(CancellationToken cancellationToken)
            {
                _handle.SetCanceled(cancellationToken);
            }
        }

        #endregion
    }
}
