using UnityEngine.Events;

namespace Framework
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