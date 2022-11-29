using System;

namespace Framework
{
    public interface IObservable
    {
        void AddRawListener(Action<object> listener);
        object RawValue { get; }
        Type RawType { get; }
        void InitValueWithoutCb(object val);
        void ForceTrigger();
    }

    public struct UnRegister : IClearable , IDisposable
    {
        private Action action;

        public UnRegister(Action action)
        {
            this.action = action;
        }

        public void Clear()
        {
            action();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}