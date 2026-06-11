using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityRequestQueue.Runtime.AssetManagement;
using UnityRequestQueue.Runtime.Factories;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace UnityRequestQueue.Runtime.Presentation
{
    public sealed class PresenterFactory : IAsyncFactory<PresenterRequest, PresenterHandle>
    {
        private readonly IAssetProvider _assetProvider;
        private readonly DiContainer _container;

        [Preserve]
        public PresenterFactory(
            IAssetProvider assetProvider,
            DiContainer container)
        {
            _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async UniTask<PresenterHandle> CreateAsync(
            PresenterRequest param,
            CancellationToken cancellationToken)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            GameObject instance = null;
            IPresenter presenter = null;

            try
            {
                instance = await _assetProvider.InstantiateAsync(
                    param.AssetKey,
                    param.Parent,
                    cancellationToken,
                    param.InstantiateInWorldSpace);

                var view = FindView(instance);
                var binding = GetBinding(view);
                presenter = CreatePresenter(binding);
                var model = param.Model ?? CreateModel(binding);

                AttachPresenter(presenter, view, param.Parameters, model);
                return new PresenterHandle(_assetProvider, instance, view, presenter);
            }
            catch
            {
                presenter?.Dispose();

                if (instance != null)
                {
                    _assetProvider.ReleaseInstance(instance);
                }

                throw;
            }
        }

        private static IView FindView(GameObject instance)
        {
            var view = instance
                .GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<IView>()
                .FirstOrDefault();

            if (view == null)
            {
                throw new InvalidOperationException(
                    $"Presenter prefab '{instance.name}' does not contain a component implementing '{nameof(IView)}'.");
            }

            return view;
        }

        private static PresenterBindingAttribute GetBinding(IView view)
        {
            var binding = PresenterBindingAttribute.GetBinding(view.GetType());
            if (binding == null)
            {
                throw new InvalidOperationException(
                    $"View '{view.GetType().FullName}' is missing '{nameof(PresenterBindingAttribute)}'.");
            }

            return binding;
        }

        private IPresenter CreatePresenter(PresenterBindingAttribute binding)
        {
            var presenter = _container.Instantiate(binding.PresenterType) as IPresenter;
            if (presenter == null)
            {
                throw new InvalidOperationException(
                    $"Presenter '{binding.PresenterType.FullName}' could not be created.");
            }

            return presenter;
        }

        private object CreateModel(PresenterBindingAttribute binding)
        {
            return binding.ModelType == null ? null : _container.Instantiate(binding.ModelType);
        }

        private static void AttachPresenter(IPresenter presenter, IView view, object parameters, object model)
        {
            if (presenter is not IPresenterLifecycle lifecycle)
            {
                return;
            }

            lifecycle.Attach(view, parameters, model);
            lifecycle.OnViewBuilt();
        }
    }
}
