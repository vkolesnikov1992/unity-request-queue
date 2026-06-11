using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Network;
using UnityRequestQueue.Runtime.Presentation;

namespace UnityRequestQueue.Runtime.Features.Core
{
    public abstract class NetworkTabPresenterBase<TView, TParameters> : TabPresenterBase<TView, TParameters>
        where TView : class, IView
        where TParameters : class
    {
        private readonly IRequestQueue _requestQueue;

        protected NetworkTabPresenterBase(IRequestQueue requestQueue)
        {
            _requestQueue = requestQueue ?? throw new ArgumentNullException(nameof(requestQueue));
        }

        protected RequestScope RequestScope { get; private set; }

        protected sealed override async UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            RequestScope = new RequestScope($"{TabId}-tab");

            try
            {
                await OnNetworkTabEnterAsync(cancellationToken);
            }
            catch
            {
                CancelRequests();
                throw;
            }
        }

        protected sealed override async UniTask OnTabExitAsync(CancellationToken cancellationToken)
        {
            CancelRequests();
            await OnNetworkTabExitAsync(cancellationToken);
        }

        protected void CancelRequests()
        {
            if (RequestScope == null)
            {
                return;
            }

            _requestQueue.Cancel(RequestScope);
            RequestScope.Dispose();
            RequestScope = null;
        }

        protected virtual UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnNetworkTabExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnDispose()
        {
            CancelRequests();
        }
    }

    public abstract class NetworkTabPresenterBase<TView, TParameters, TModel> : TabPresenterBase<TView, TParameters, TModel>
        where TView : class, IView
        where TParameters : class
        where TModel : class
    {
        private readonly IRequestQueue _requestQueue;

        protected NetworkTabPresenterBase(IRequestQueue requestQueue)
        {
            _requestQueue = requestQueue ?? throw new ArgumentNullException(nameof(requestQueue));
        }

        protected RequestScope RequestScope { get; private set; }

        protected sealed override async UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            RequestScope = new RequestScope($"{TabId}-tab");

            try
            {
                await OnNetworkTabEnterAsync(cancellationToken);
            }
            catch
            {
                CancelRequests();
                throw;
            }
        }

        protected sealed override async UniTask OnTabExitAsync(CancellationToken cancellationToken)
        {
            CancelRequests();
            await OnNetworkTabExitAsync(cancellationToken);
        }

        protected void CancelRequests()
        {
            if (RequestScope == null)
            {
                return;
            }

            _requestQueue.Cancel(RequestScope);
            RequestScope.Dispose();
            RequestScope = null;
        }

        protected virtual UniTask OnNetworkTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnNetworkTabExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnDispose()
        {
            CancelRequests();
            base.OnDispose();
        }
    }
}
