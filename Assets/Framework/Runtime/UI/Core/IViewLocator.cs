using System;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.UI.Core
{
    public interface IViewLocator
    {
        [Obsolete("use LoadViewAsync replace", true)]
        View LoadView(string path, ViewModel viewModel = null, bool autoShow = true);
        [Obsolete("use LoadViewAsync replace", true)]
        T LoadView<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View;
        IProgressResult<float, View> LoadViewAsync(string path, ViewModel viewModel = null, bool autoShow = true);
        IProgressResult<float, T> LoadViewAsync<T>(string path, ViewModel viewModel = null, bool autoShow = true) where T : View;
    }
}