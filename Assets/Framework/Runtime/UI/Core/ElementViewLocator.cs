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

        public View Load(string path, ViewModel viewModel = null)
        {
            var trans = UIEnv.LoadPrefabFunc(path);
            trans = Object.Instantiate(trans);
            trans.transform.SetParent(content, false);
            var view = trans.GetComponent<View>();
            view.SetVm(viewModel);
            return view;
        }
    }
}