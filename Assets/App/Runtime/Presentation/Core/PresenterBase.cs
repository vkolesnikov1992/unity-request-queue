using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace UnityRequestQueue.Runtime.Presentation
{
    internal interface IPresenterLifecycle
    {
        void Attach(IView view, object parameters, object model);

        void OnViewBuilt();
    }

    public abstract class PresenterBase<TView> : IPresenter, IPresenterLifecycle
        where TView : class, IView
    {
        private readonly CancellationTokenSource _disposeCancellation = new();
        private CancellationTokenSource _activationCancellation;
        private bool _isEntered;
        private bool _disposed;

        protected TView View { get; private set; }

        protected CancellationToken LifetimeToken => _disposeCancellation.Token;

        protected CancellationToken ActivationToken =>
            _activationCancellation?.Token ?? CancellationToken.None;

        void IPresenterLifecycle.Attach(IView view, object parameters, object model)
        {
            ThrowIfDisposed();

            if (view is not TView typedView)
            {
                throw new InvalidOperationException(
                    $"Presenter '{GetType().FullName}' requires view '{typeof(TView).FullName}', received '{view?.GetType().FullName ?? "null"}'.");
            }

            View = typedView;
            OnAttached(typedView, parameters, model);
        }

        void IPresenterLifecycle.OnViewBuilt()
        {
            ThrowIfDisposed();
            OnViewBuilt();
        }

        public async UniTask EnterAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (_isEntered)
            {
                return;
            }

            _isEntered = true;
            _activationCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                _disposeCancellation.Token,
                cancellationToken);

            try
            {
                await OnEnterAsync(_activationCancellation.Token);
            }
            catch
            {
                _isEntered = false;
                CancelActivation();
                DisposeActivation();
                throw;
            }
        }

        public async UniTask ExitAsync(CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (!_isEntered)
            {
                return;
            }

            _isEntered = false;
            CancelActivation();

            try
            {
                await OnExitAsync(cancellationToken);
            }
            finally
            {
                DisposeActivation();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            CancelActivation();
            _disposeCancellation.Cancel();

            try
            {
                OnDispose();
            }
            finally
            {
                DisposeActivation();
                _disposeCancellation.Dispose();
                View = null;
            }
        }

        protected virtual void OnAttached(TView view, object parameters, object model)
        {
        }

        protected virtual void OnViewBuilt()
        {
        }

        protected virtual UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnExitAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual void OnDispose()
        {
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void CancelActivation()
        {
            if (_activationCancellation != null && !_activationCancellation.IsCancellationRequested)
            {
                _activationCancellation.Cancel();
            }
        }

        private void DisposeActivation()
        {
            _activationCancellation?.Dispose();
            _activationCancellation = null;
        }
    }

    public abstract class PresenterBase<TView, TParameters> : PresenterBase<TView>
        where TView : class, IView
        where TParameters : class
    {
        protected TParameters Parameters { get; private set; }

        protected virtual TParameters DefaultParameters => null;

        protected sealed override void OnAttached(TView view, object parameters, object model)
        {
            Parameters = ResolveParameters(parameters);
            OnModelAttached(view, model);
        }

        protected virtual void OnModelAttached(TView view, object model)
        {
            OnAttached(view);
        }

        protected virtual void OnAttached(TView view)
        {
        }

        private TParameters ResolveParameters(object parameters)
        {
            if (parameters is TParameters typedParameters)
            {
                return typedParameters;
            }

            if (parameters == null && DefaultParameters != null)
            {
                return DefaultParameters;
            }

            if (parameters == null)
            {
                throw new InvalidOperationException(
                    $"Presenter '{GetType().Name}' requires parameters of type '{typeof(TParameters).Name}'.");
            }

            throw new InvalidOperationException(
                $"Presenter '{GetType().Name}' received parameters of type '{parameters.GetType().Name}' instead of '{typeof(TParameters).Name}'.");
        }
    }

    public abstract class PresenterBase<TView, TParameters, TModel> : PresenterBase<TView, TParameters>
        where TView : class, IView
        where TParameters : class
        where TModel : class
    {
        protected TModel Model { get; private set; }

        protected virtual TModel DefaultModel => null;

        protected sealed override void OnModelAttached(TView view, object model)
        {
            Model = ResolveModel(model);
            OnAttached(view);
        }

        protected override void OnDispose()
        {
            Model = null;
        }

        private TModel ResolveModel(object model)
        {
            if (model is TModel typedModel)
            {
                return typedModel;
            }

            if (model == null && DefaultModel != null)
            {
                return DefaultModel;
            }

            if (model == null)
            {
                throw new InvalidOperationException(
                    $"Presenter '{GetType().Name}' requires model of type '{typeof(TModel).Name}'.");
            }

            throw new InvalidOperationException(
                $"Presenter '{GetType().Name}' received model of type '{model.GetType().Name}' instead of '{typeof(TModel).Name}'.");
        }
    }
}
