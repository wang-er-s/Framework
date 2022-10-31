using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class DefaultBindList<TComponent, TVm> : IBindList<TVm> where TComponent :  Component
    {
        private List<TComponent> _allObj = new List<TComponent>();
        private TComponent _template;
        private Transform _parent;
        private Action<TComponent, TVm> _onCreate;
        private Action<TComponent, TVm> _onDestroy;

        public DefaultBindList(TComponent template, Action<TComponent, TVm> onCreate, Action<TComponent, TVm> onDestroy)
        {
            _template = template;
            _parent = template.transform.parent;
            _onCreate = onCreate;
            _onDestroy = onDestroy;
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
                    _onCreate?.Invoke(gen, obj);
                    _allObj.Add(gen);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    _onDestroy?.Invoke(_allObj[index], obj);
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

#pragma warning disable 693
        public static DefaultBindList<Component, TVm> Create<TObject,TVm>(Object component, Action<TObject, TVm> onCreate, Action<TObject, TVm> onDestroy) where TObject : Object
#pragma warning restore 693
        {
            Log.Assert(typeof(TObject).IsSubclassOf(typeof(Component)), "typeof(TObject) Is not SubclassOf(typeof(Component))");
            return new DefaultBindList<Component, TVm>(component as Component, (component1, vm) =>
            {
                onCreate?.Invoke(component1 as TObject, vm);
            }, (component1, vm) =>
            {
                onDestroy?.Invoke(component1 as TObject, vm);
            });
        }
    }
}