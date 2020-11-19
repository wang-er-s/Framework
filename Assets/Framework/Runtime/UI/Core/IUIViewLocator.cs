using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IUIViewLocator
    {
        T Load<T>(string path, ViewModel viewModel = null) where T : View;
    }
}