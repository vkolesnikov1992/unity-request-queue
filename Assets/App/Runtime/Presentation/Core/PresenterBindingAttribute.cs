using System;

namespace UnityRequestQueue.Runtime.Presentation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PresenterBindingAttribute : Attribute
    {
        public PresenterBindingAttribute(Type presenter, Type model = null)
        {
            if (presenter == null)
            {
                throw new ArgumentNullException(nameof(presenter));
            }

            if (!typeof(IPresenter).IsAssignableFrom(presenter))
            {
                throw new ArgumentException(
                    $"Presenter '{presenter.FullName}' must implement '{nameof(IPresenter)}'.",
                    nameof(presenter));
            }

            PresenterType = presenter;
            ModelType = model;
        }

        public Type PresenterType { get; }

        public Type ModelType { get; }

        public static PresenterBindingAttribute GetBinding(Type viewType)
        {
            if (viewType == null)
            {
                throw new ArgumentNullException(nameof(viewType));
            }

            if (!typeof(IView).IsAssignableFrom(viewType))
            {
                throw new InvalidOperationException(
                    $"Type '{viewType.FullName}' is not a view.");
            }

            var binding = Attribute.GetCustomAttribute(viewType, typeof(PresenterBindingAttribute)) as PresenterBindingAttribute;
            return binding;
        }
    }
}
