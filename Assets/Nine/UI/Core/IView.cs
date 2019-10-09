using System;
using UnityEngine;

namespace Nine.UI.Core
{
    public interface IView
    {
        void Create(ViewModel vm);
        void Close();
        void Show();
        void Hide();
    }

}