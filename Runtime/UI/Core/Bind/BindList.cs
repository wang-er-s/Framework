using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using Framework.UI.Wrap;
using Framework.UI.Wrap.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Core.Bind
{
    public class BindList<TComponent,TVm> : BaseBind where TComponent : UnityEngine.Object
    {
        private readonly TComponent _component;
        private readonly ObservableList<TVm> _list;
        private IBindList<TVm> _bindList;

        public BindList(object container, TComponent component, ObservableList<TVm> list, Action<TComponent, TVm> onShow, Action<TComponent, TVm> onHide) : base(container)
        {
            _component = component;
            this._list = list;
            InitEvent(onShow, onHide);
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

        private void InitEvent(Action<TComponent, TVm> onShow, Action<TComponent, TVm> onHide)
        {
            _bindList = _bindList ?? _component as IBindList<TVm> ??
                DefaultBindList<Component, TVm>.Create(_component, onShow, onHide);
            Log.Assert(_bindList != null, $"can not find IBindList of {_component}");
            _list.AddListener(_bindList.GetBindListFunc());
        }

        public override void ClearBind()
        {
            _list.ClearListener();
        }
    }
}