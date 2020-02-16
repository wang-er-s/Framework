using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IView
    {
        ViewModel ViewModel { get; }
        Transform Transform { get; }
        UIManager UiManager { get; }
        void Show();
        void Hide();
        void SetVM(ViewModel viewModel);
        void SetUIManager(UIManager uiManager);
    }

}