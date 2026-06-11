using Cysharp.Threading.Tasks;
using System.Threading;
using UnityRequestQueue.Runtime.Presentation;

namespace UnityRequestQueue.Runtime.Features.Core
{
    public abstract class TabPresenterBase<TView, TParameters> : PresenterBase<TView, TParameters>
        where TView : class, IView
        where TParameters : class
    {
        public abstract TabId TabId { get; }

        protected bool IsActive { get; private set; }

        protected sealed override async UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            IsActive = true;
            View.SetVisible(true);
            await OnTabEnterAsync(cancellationToken);
        }

        protected sealed override async UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            await OnTabExitAsync(cancellationToken);
            View.SetVisible(false);
            IsActive = false;
        }

        protected virtual UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnTabExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }

    public abstract class TabPresenterBase<TView, TParameters, TModel> : PresenterBase<TView, TParameters, TModel>
        where TView : class, IView
        where TParameters : class
        where TModel : class
    {
        public abstract TabId TabId { get; }

        protected bool IsActive { get; private set; }

        protected sealed override async UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            IsActive = true;
            View.SetVisible(true);
            await OnTabEnterAsync(cancellationToken);
        }

        protected sealed override async UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            await OnTabExitAsync(cancellationToken);
            View.SetVisible(false);
            IsActive = false;
        }

        protected virtual UniTask OnTabEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnTabExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }
}
