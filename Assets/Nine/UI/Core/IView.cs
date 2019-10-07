using System;
using UnityEngine;

namespace Nine.UI.Core
{
    public interface IView<T> where T : ViewModel
    {
        T Data { get; }
        void Create(T vm);
        void Close();
        void Show();
        void Hide();
    }
}