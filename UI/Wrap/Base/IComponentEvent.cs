using UnityEngine.Events;

namespace AD.UI.Wrap
{
    public interface IComponentEvent<T>
    {
        UnityEvent<T> GetComponentEvent();
    }

    public interface IComponentEvent
    {
        UnityEvent GetComponentEvent();
    }
}
