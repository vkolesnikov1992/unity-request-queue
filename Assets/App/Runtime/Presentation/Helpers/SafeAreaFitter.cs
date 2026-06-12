using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation.Helpers
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _target;

        private Rect _lastSafeArea;
        private Vector2Int _lastScreenSize;
        private ScreenOrientation _lastOrientation;

        private RectTransform Target
        {
            get
            {
                if (!_target)
                {
                    _target = transform as RectTransform;
                }

                return _target;
            }
        }

        private void OnEnable()
        {
            ApplySafeArea(force: true);
        }

        private void Update()
        {
            ApplySafeArea(force: false);
        }

        private void OnRectTransformDimensionsChange()
        {
            ApplySafeArea(force: false);
        }

        #if !UNITY_EDITOR
        private void OnValidate()
        {
            ApplySafeArea(force: true);
        }
        #endif

        private void ApplySafeArea(bool force)
        {
            var target = Target;
            
            if (!target)
            {
                return;
            }

            var safeArea = Screen.safeArea;
            var screenSize = new Vector2Int(Screen.width, Screen.height);
            var orientation = Screen.orientation;

            if (!force
                && safeArea == _lastSafeArea
                && screenSize == _lastScreenSize
                && orientation == _lastOrientation)
            {
                return;
            }

            _lastSafeArea = safeArea;
            _lastScreenSize = screenSize;
            _lastOrientation = orientation;

            if (screenSize.x <= 0 || screenSize.y <= 0)
            {
                return;
            }

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenSize.x;
            anchorMin.y /= screenSize.y;
            anchorMax.x /= screenSize.x;
            anchorMax.y /= screenSize.y;

            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
            target.offsetMin = Vector2.zero;
            target.offsetMax = Vector2.zero;
        }
    }
}
