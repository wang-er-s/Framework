using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nine.UI.Core;
using UnityEngine;

namespace Assets.Nine.UI.Core
{
    public class BindList<TVm> : IBindSet where TVm : ViewModel, new()
    {
        private Transform content;
        private Transform template;
        private BindableList<TVm> list;

        public BindList(View<TVm> view, BindableList<TVm> _list)
        {
            template = view.transform;
            content = template.parent;
            list = _list;
        }


        void IBindSet.Init()
        {
            //list.AddListener();
        }

    }
}
