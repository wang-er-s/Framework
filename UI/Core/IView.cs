using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IView
    {
        ViewModel VM { get; set; }
        void Create();
        void Destroy();
        void Show();
        void Hide();
    }

}