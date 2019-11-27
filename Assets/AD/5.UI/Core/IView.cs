using System;
using UnityEngine;

namespace AD.UI.Core
{
    public interface IView
    {
        void Create(ViewModel vm);
        void Destroy();
        void Show();
        void Hide();
    }

}