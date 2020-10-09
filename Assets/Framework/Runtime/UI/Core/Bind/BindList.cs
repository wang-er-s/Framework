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
        private readonly TComponent _component;
        private readonly ObservableList<TVm> _list;
        private IBindList<TVm> _bindList;

        public BindList(TComponent view, ObservableList<TVm> list)
        {
            _component = view;
            this._list = list;
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            _bindList.GetBindListFunc()(NotifyCollectionChangedAction.Reset, default, -1);
            for (var i = 0; i < _list.Count; i++)
            {
                _bindList.GetBindListFunc()(NotifyCollectionChangedAction.Add, _list[i], i);
            }
        }

        private void InitEvent()
        {
            if (_bindList == null)
            {
                _bindList = _component as IBindList<TVm>;
            }
            if (_bindList == null)
            {
                var bind = BindTool.GetDefaultWrapper(_component);
                _bindList = bind as IBindList<TVm>;
            }
            Log.Assert(_bindList != null);
            if(_bindList == null) return;
            _list.AddListener(_bindList.GetBindListFunc());
        }
    }
}