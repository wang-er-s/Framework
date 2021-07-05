using System;

namespace Framework.UI.Core.Bind
{
    public interface IObservable
    {
        void AddListener(Action<object> changeAction);

        void Clear();
    }
}