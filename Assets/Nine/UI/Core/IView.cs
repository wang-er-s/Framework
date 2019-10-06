using System;
using UnityEngine;

namespace Nine.UI.Core
{
    public interface IView<out T> where T : ViewModelBase
    {
        T Data { get; }
        void Create(bool immediate=false,Action<Transform> action=null);
        void Close(bool immediate=false,Action<Transform> action=null);
    }
}