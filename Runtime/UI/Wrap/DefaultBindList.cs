using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Framework.UI.Wrap.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class DefaultBindList<TComponent, TVm> : IBindList<TVm> where TComponent :  UnityEngine.Object
    {
        private List<TComponent> _allObj = new List<TComponent>();
        private TComponent _template;
        private Action<TComponent, TVm> _onShow;
        private Action<TComponent, TVm> _onHide;

        public DefaultBindList(TComponent template, Action<TComponent, TVm> onShow, Action<TComponent, TVm> onHide)
        {
            _template = template;
            _onShow = onShow;
            _onHide = onHide;
        }

        public Action<NotifyCollectionChangedAction, TVm, int> GetBindListFunc()
        {
            return BindListFunc;
        }

        private void BindListFunc
            (NotifyCollectionChangedAction type, TVm obj, int index)
        {
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    var gen = Object.Instantiate(_template);
                    _onShow?.Invoke(gen, obj);
                    _allObj.Add(gen);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _onHide?.Invoke(_allObj[index], obj);
                    Object.Destroy(_allObj[index]);
                    _allObj.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Log.Warning("default bind list not support replace");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _allObj.ForEach(Object.Destroy);
                    _allObj.Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}