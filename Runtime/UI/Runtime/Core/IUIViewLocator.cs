using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IUIViewLocator
    {
        View Load(string path, ViewModel viewModel = null);
    }
}