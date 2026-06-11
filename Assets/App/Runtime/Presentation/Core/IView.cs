using UnityEngine;

namespace UnityRequestQueue.Runtime.Presentation
{
    public interface IView
    {
        GameObject GameObject { get; }

        Transform Transform { get; }

        void SetVisible(bool visible);
    }
}
