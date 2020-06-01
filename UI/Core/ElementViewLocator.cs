using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public class ElementViewLocator : IUIViewLocator
    {
        public ElementViewLocator(Transform content)
        {
            _content = content;
        }

        private Transform _content;
        public Func<string, GameObject> LoadResFunc { get; set; } = null;
        
        public View Load(string path, ViewModel viewModel = null)
        {
            var trans = LoadResFunc?.Invoke(path).transform;
            trans.SetParent(_content, false);
            var view = trans.GetComponent<View>();
            view.SetVM(viewModel);
            return view;
        }
    }
}