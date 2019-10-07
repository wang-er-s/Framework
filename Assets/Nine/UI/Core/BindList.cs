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
        private View<TVm> view;
        private BindableList<TVm> list;
        private BaseWrapper<View<TVm>> wrapper;
        private IBindList<TVm> bindList;

        public BindList(View<TVm> _view, BindableList<TVm> _list)
        {
            view = _view;
            content = view.transform.parent;
            list = _list;
        }


        public void Init()
        {
            //TODO ItemView<ItemViewModel> 与 View<ViewModel> 是两个类，不能获取到Wrapper，考虑替换掉View的泛型生成VM
            wrapper = WrapTool.GetWrapper(view);
            bindList = wrapper as IBindList<TVm>;
            list.AddListener(bindList.GetBindListFunc());
        }

    }
}
