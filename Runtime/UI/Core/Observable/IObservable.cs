using System;

namespace Framework
{
    public interface IObservable
    {
        void AddRawListener(Action<object> listener);
        object RawValue { get; }
        Type RawType { get; }
        void InitRawValueWithoutCb(object val);
        void ForceTrigger();
    }

    public struct UnRegister : IResetBind
    {
        private Action action;

        public UnRegister(Action action)
        {
            this.action = action;
        }

        public void Invoke()
        {
            action();
        }

        public void Reset()
        {
            Invoke();
        }
    }
}