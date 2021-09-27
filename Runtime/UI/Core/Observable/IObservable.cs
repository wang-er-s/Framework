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

    public struct UnRegister : IClearable
    {
        private Action action;

        public UnRegister(Action action)
        {
            this.action = action;
        }
        
        public void ClearView()
        {
            
        }

        public void ClearModel()
        {
            action();
        }
    }
}