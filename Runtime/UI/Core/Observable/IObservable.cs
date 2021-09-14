using System;

namespace Framework.UI.Core.Bind
{
    public interface IObservable
    {
        void AddListener(Action<object> listener);
        object RawValue { get; }
        Type RawType { get; }
        void InitValueWithoutCb(object val);
        void ForceTrigger();
    }
}