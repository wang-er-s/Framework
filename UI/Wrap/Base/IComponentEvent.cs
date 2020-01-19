using UnityEngine.Events;

namespace Framework.UI.Wrap
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
