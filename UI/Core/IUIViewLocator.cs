using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IUIViewLocator
    {
        Func<string, GameObject> LoadResFunc { get; set; }
        View Load(string path, ViewModel viewModel = null);
    }
}