using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace UnityRequestQueue.Runtime.Bootstrap
{
    public sealed class AppStartupPipeline
    {
        private readonly IReadOnlyList<IAppStartupStep> _steps;

        public AppStartupPipeline(IEnumerable<IAppStartupStep> steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            _steps = steps
                .OrderBy(step => step.Stage)
                .ThenBy(step => step.Order)
                .ThenBy(step => step.Name, StringComparer.Ordinal)
                .ToArray();
        }

        public async UniTask RunAsync(AppStartupContext context, CancellationToken cancellationToken)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Debug.Log($"App startup pipeline: {string.Join(" -> ", _steps.Select(FormatStep))}");

            foreach (var step in _steps)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Debug.Log($"App startup step started: {FormatStep(step)}");

                await step.RunAsync(context, cancellationToken);

                Debug.Log($"App startup step finished: {FormatStep(step)}");
            }
        }

        private static string FormatStep(IAppStartupStep step)
        {
            return $"{step.Stage}:{step.Order}:{step.Name}";
        }
    }
}
