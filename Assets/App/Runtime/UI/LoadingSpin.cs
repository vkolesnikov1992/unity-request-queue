using UnityEngine;

namespace UnityRequestQueue.Runtime.UI
{
    [DisallowMultipleComponent]
    public sealed class LoadingSpin : MonoBehaviour
    {
        [SerializeField]
        private Vector3 _axis = Vector3.forward;
        [SerializeField]
        private float _degreesPerSecond = -240f;
        [SerializeField]
        private bool _useUnscaledTime = true;

        private void Update()
        {
            var deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            var angle = _degreesPerSecond * deltaTime;

            if (transform is RectTransform rectTransform)
            {
                var center = rectTransform.TransformPoint(rectTransform.rect.center);
                transform.RotateAround(center, transform.TransformDirection(_axis), angle);
                return;
            }

            transform.Rotate(_axis, angle, Space.Self);
        }
    }
}
