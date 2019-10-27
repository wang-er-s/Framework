using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nine.UI.Wrap;
using Nine.UI.Core;
using UnityEngine;

namespace Assets.Nine.UI.Core
{
    public class BindList<TVm> : IBindSet where TVm : ViewModel
    {
        private Transform content;
        private View view;
        private BindableList<TVm> list;
        private BaseWrapper<View> wrapper;
        private IBindList<ViewModel> bindList;

        public BindList(View _view, BindableList<TVm> _list)
        {
            view = _view;
            content = view.transform.parent;
            list = _list;
            Init();
        }
        
        private void Init()
        {
            wrapper = WrapTool.GetWrapper(view);
            bindList = wrapper as IBindList<ViewModel>;
            list.AddListener(bindList.GetBindListFunc());
        }

    }
}
