using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation.Helpers
{
    [DisallowMultipleComponent]
    public sealed class TransformRotator : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;
        [SerializeField]
        private Vector3 _axis = Vector3.forward;
        [SerializeField]
        private float _degreesPerSecond = -240f;
        [SerializeField]
        private bool _useUnscaledTime = true;

        private Transform Target => _target ? _target : transform;

        public void SetSpeed(float degreesPerSecond)
        {
            _degreesPerSecond = degreesPerSecond;
        }

        private void Update()
        {
            var target = Target;

            if (!target)
            {
                return;
            }

            var deltaTime = _useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            target.Rotate(_axis, _degreesPerSecond * deltaTime, Space.Self);
        }
    }
}
