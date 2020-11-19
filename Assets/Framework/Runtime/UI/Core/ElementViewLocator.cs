using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core
{
    public class ElementViewLocator : IUIViewLocator
    {
        public ElementViewLocator(Transform content)
        {
            this.content = content;
        }

        private Transform content;
        
        public T Load<T>(string path, ViewModel viewModel = null) where T : View
        {
            var trans = UIEnv.LoadPrefabFunc(path);
            trans = Object.Instantiate(trans, content, false);
            var view = trans.GetComponent<T>();
            view.SetVm(viewModel);
            return view;
        }
    }
}