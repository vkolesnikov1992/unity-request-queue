using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public class Application : IAppEntryPoint
    {
        private readonly DiContainer _container;

        [Preserve]
        public Application(DiContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async UniTask RunAsync(CancellationToken cancellationToken)
        {
            Debug.Log("Application startup started.");

            await RunStageAsync("Load", LoadAsync, cancellationToken);
            await RunStageAsync("StartupPipeline", RunStartupPipelineAsync, cancellationToken);
            await RunStageAsync("Started", OnStartedAsync, cancellationToken);

            Debug.Log("Application startup finished.");
        }

        protected virtual UniTask LoadAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask RunStartupPipelineAsync(CancellationToken cancellationToken)
        {
            var context = _container.Resolve<AppStartupContext>();
            var pipeline = _container.Resolve<AppStartupPipeline>();

            return pipeline.RunAsync(context, cancellationToken);
        }

        protected virtual UniTask OnStartedAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        private static async UniTask RunStageAsync(
            string name,
            Func<CancellationToken, UniTask> stage,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Debug.Log($"Application stage started: {name}");

            await stage(cancellationToken);

            Debug.Log($"Application stage finished: {name}");
        }
    }
}
