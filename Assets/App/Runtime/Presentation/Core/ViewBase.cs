using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation
{
    public abstract class ViewBase : MonoBehaviour, IView
    {
        public GameObject GameObject => gameObject;

        public Transform Transform => transform;

        public virtual void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
