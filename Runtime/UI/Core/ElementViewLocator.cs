using System;
using UnityEngine;

namespace Framework.UI.Core
{
    public class ElementViewLocator : IUIViewLocator
    {
        public ElementViewLocator(Transform content)
        {
            this.content = content;
        }

        private Transform content;

        public View Load(string path, ViewModel viewModel = null)
        {
            var trans = UIEnv.LoadPrefabFunc(path).transform;
            trans.SetParent(content, false);
            var view = trans.GetComponent<View>();
            view.SetVm(viewModel);
            return view;
        }
    }
}