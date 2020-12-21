using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Framework.UI.Wrap.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class DefaultBindList<TComponent, TVm> : IBindList<TVm> where TComponent :  Component
    {
        private List<TComponent> _allObj = new List<TComponent>();
        private TComponent _template;
        private Transform _parent;
        private Action<TComponent, TVm> _onShow;
        private Action<TComponent, TVm> _onHide;

        public DefaultBindList(TComponent template, Action<TComponent, TVm> onShow, Action<TComponent, TVm> onHide)
        {
            _template = template;
            _parent = template.transform.parent;
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

                    var gen = Object.Instantiate(_template, _parent, false);
                    _onShow?.Invoke(gen, obj);
                    _allObj.Add(gen);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _onHide?.Invoke(_allObj[index], obj);
                    Object.Destroy(_allObj[index].gameObject);
                    _allObj.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Log.Warning("default bind list not support replace");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _allObj.ForEach(com => Object.Destroy(com.gameObject));
                    _allObj.Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static DefaultBindList<Component, TVm> Create<TObject,TVm>(Object component, Action<TObject, TVm> onShow, Action<TObject, TVm> onHide) where TObject : Object
        {
            Log.Assert(typeof(TObject).IsSubclassOf(typeof(Component)), "typeof(TObject) Is not SubclassOf(typeof(Component))");
            return new DefaultBindList<Component, TVm>(component as Component, (component1, vm) =>
            {
                onShow?.Invoke(component1 as TObject, vm);
            }, (component1, vm) =>
            {
                onHide?.Invoke(component1 as TObject, vm);
            });
        }
    }
}