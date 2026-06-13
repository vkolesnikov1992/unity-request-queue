using System;
using UnityEngine;

namespace UnityRequestQueue.Runtime.UI
{
    public sealed class UICanvasRoot
    {
        public UICanvasRoot(Canvas canvas)
        {
            Canvas = canvas ? canvas : throw new ArgumentNullException(nameof(canvas));
        }

        public Canvas Canvas { get; }

        public Transform Transform => Canvas.transform;

        public static UICanvasRoot FindInScene()
        {
            var canvas = UnityEngine.Object.FindFirstObjectByType<Canvas>();

            if (!canvas)
            {
                throw new InvalidOperationException(
                    $"Scene does not contain '{nameof(Canvas)}'. Add a Canvas before starting UI.");
            }

            return new UICanvasRoot(canvas);
        }
    }
}
