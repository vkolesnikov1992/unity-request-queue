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
        private readonly List<RequestItem> _pending = new List<RequestItem>();

        private RequestItem _current;
        private CancellationTokenSource _currentCancellation;
        private bool _isProcessing;

        [Preserve]
        public RequestQueue(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public int PendingCount => _pending.Count;

        public bool IsRunning => _current != null;

        public RequestHandle<TResponse> Enqueue<TResponse>(
            IRequestCommand<TResponse> command,
            RequestScope scope = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var item = new RequestItem<TResponse>(command, scope, Cancel);

            if (scope != null && scope.IsCancellationRequested)
            {
                item.Cancel(scope.Token);
                return item.TypedHandle;
            }

            _pending.Add(item);
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
                _currentCancellation?.Cancel();
                return;
            }

            var item = _pending.FirstOrDefault(pending => pending.Handle.Id == handle.Id);

            if (item == null)
            {
                return;
            }

            _pending.Remove(item);
            item.Cancel(item.CancellationToken);
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
                item.Cancel(scope.Token);
                canceledCount++;
            }

            if (_current != null && _current.Scope == scope)
            {
                _currentCancellation?.Cancel();
                canceledCount++;
            }

            return canceledCount;
        }

        public void Clear()
        {
            foreach (var item in _pending)
            {
                item.Cancel(item.CancellationToken);
            }

            _pending.Clear();
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
                    item.Cancel(item.CancellationToken);
                    continue;
                }

                _current = item;
                _currentCancellation = item.CreateCancellation();
                item.SetRunning();

                try
                {
                    await item.ExecuteAsync(_httpClient, _currentCancellation.Token);
                }
                catch (OperationCanceledException)
                {
                    item.Cancel(_currentCancellation.Token);
                }
                catch (Exception exception)
                {
                    item.Fail(exception);
                }
                finally
                {
                    _currentCancellation.Dispose();
                    _currentCancellation = null;
                    _current = null;
                }
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
                Action<RequestHandle> cancel)
                : base(scope)
            {
                _command = command ?? throw new ArgumentNullException(nameof(command));
                _handle = new RequestHandle<TResponse>(
                    Guid.NewGuid(),
                    command.Name,
                    scope,
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
