using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityRequestQueue.Runtime.Factories;
using UnityRequestQueue.Runtime.Presentation;

namespace UnityRequestQueue.Runtime.Features.Main
{
    public sealed class MainPresenter : PresenterBase<MainView, MainParameters>
    {
        private readonly IAsyncFactory<PresenterRequest, PresenterHandle> _presenterFactory;

        public MainPresenter(IAsyncFactory<PresenterRequest, PresenterHandle> presenterFactory)
        {
            _presenterFactory = presenterFactory ?? throw new ArgumentNullException(nameof(presenterFactory));
        }

        protected override MainParameters DefaultParameters => MainParameters.Default;

        protected override UniTask OnEnterAsync(CancellationToken cancellationToken)
        {
            View.Tabs.Initialize(Parameters.InitialTab);
            return UniTask.CompletedTask;
        }
    }
}
