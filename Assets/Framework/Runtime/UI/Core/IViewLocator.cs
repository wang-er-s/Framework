using System;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IViewLocator
    {
        View LoadView(string path, ViewModel viewModel = null, bool autoShow = true);
        T LoadView<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View;
        IProgressResult<float, View> LoadViewAsync(string path, ViewModel viewModel = null, bool autoShow = true);
        IProgressResult<float, T> LoadViewAsync<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View;
    }
}