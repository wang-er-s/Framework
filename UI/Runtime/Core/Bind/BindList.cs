using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.UI.Wrap;
using Framework.UI.Wrap.Base;
using UnityEngine;

namespace Framework.UI.Core.Bind
{
    public class BindList<TComponent,TVm>
    {
        private TComponent component;
        private BindableList<TVm> list;
        private IBindList<TVm> bindList;

        public BindList(TComponent view, BindableList<TVm> list)
        {
            component = view;
            this.list = list;
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            bindList.GetBindListFunc()(NotifyCollectionChangedAction.Reset, default, -1);
            for (var i = 0; i < list.Count; i++)
            {
                bindList.GetBindListFunc()(NotifyCollectionChangedAction.Add, list[i], i);
            }
        }

        private void InitEvent()
        {
            if (bindList == null)
            {
                bindList = component as IBindList<TVm>;
            }
            if (bindList == null)
            {
                var bind = BindTool.GetDefaultWrapper(component);
                bindList = bind as IBindList<TVm>;
            }
            Log.Assert(bindList != null);
            if(bindList == null) return;
            list.AddListener(bindList.GetBindListFunc());
        }
    }
}