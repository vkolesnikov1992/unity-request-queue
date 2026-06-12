using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Bootstrap;
using UnityRequestQueue.Runtime.Factories;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Presentation;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Features.Main
{
    public sealed class MainPresenterStartupStep : IAppStartupStep, IDisposable
    {
        private readonly IAsyncFactory<PresenterRequest, PresenterHandle> _presenterFactory;

        private PresenterHandle _mainHandle;

        [Preserve]
        public MainPresenterStartupStep(IAsyncFactory<PresenterRequest, PresenterHandle> presenterFactory)
        {
            _presenterFactory = presenterFactory;
        }

        public AppStartupStage Stage => AppStartupStage.Presentation;

        public int Order => 0;

        public string Name => nameof(MainPresenterStartupStep);

        public async UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            var presenterRoot = GetCanvasRoot();
            var request = new PresenterRequest(FeatureAssetKeys.Main, presenterRoot);

            _mainHandle = await _presenterFactory.CreateAsync(request, cancellationToken);
            await _mainHandle.Presenter.EnterAsync(cancellationToken);
        }

        public void Dispose()
        {
            _mainHandle?.Dispose();
            _mainHandle = null;
        }

        private static Transform GetCanvasRoot()
        {
            var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

            if (!canvas)
            {
                throw new InvalidOperationException(
                    $"Scene does not contain '{nameof(Canvas)}'. Add a Canvas before starting presentation.");
            }

            return canvas.transform;
        }
    }
}
