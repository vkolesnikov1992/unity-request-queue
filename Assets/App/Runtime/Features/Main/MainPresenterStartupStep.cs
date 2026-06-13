using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Bootstrap;
using UnityRequestQueue.Runtime.Factories;
using UnityRequestQueue.Runtime.Features.Core;
using UnityRequestQueue.Runtime.Presentation;
using UnityRequestQueue.Runtime.UI;
using UnityEngine.Scripting;

namespace UnityRequestQueue.Runtime.Features.Main
{
    public sealed class MainPresenterStartupStep : IAppStartupStep, IDisposable
    {
        private readonly IAsyncFactory<PresenterRequest, PresenterHandle> _presenterFactory;
        private readonly UICanvasRoot _canvasRoot;

        private PresenterHandle _mainHandle;

        [Preserve]
        public MainPresenterStartupStep(
            IAsyncFactory<PresenterRequest, PresenterHandle> presenterFactory,
            UICanvasRoot canvasRoot)
        {
            _presenterFactory = presenterFactory;
            _canvasRoot = canvasRoot;
        }

        public AppStartupStage Stage => AppStartupStage.Presentation;

        public int Order => 0;

        public string Name => nameof(MainPresenterStartupStep);

        public async UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            var request = new PresenterRequest(FeatureAssetKeys.Main, _canvasRoot.Transform);

            _mainHandle = await _presenterFactory.CreateAsync(request, cancellationToken);
            await _mainHandle.Presenter.EnterAsync(cancellationToken);
        }

        public void Dispose()
        {
            _mainHandle?.Dispose();
            _mainHandle = null;
        }
    }
}
