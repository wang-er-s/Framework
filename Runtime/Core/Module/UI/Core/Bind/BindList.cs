using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public class BindList<TComponent,TVm> : BaseBind where TComponent : Component
    {
        private TComponent _component;
        private ObservableList<TVm> _list;
        private List<TComponent> _allObj = new List<TComponent>();
        private Action<NotifyCollectionChangedAction, TVm, int> bindListFunc;
        private Action<TComponent, TVm> onCreate;
        private Action<TComponent, TVm> onDestroy;
        private SinglePrefabPool<TComponent> singlePrefabPool;

        private BindList()
        {
        }

        public void Reset(TComponent component, ObservableList<TVm> list, Action<TComponent, TVm> onCreate,
            Action<TComponent, TVm> onDestroy)
        {
            _component = component;
            this._list = list;
            this.onCreate = onCreate;
            this.onDestroy = onDestroy;
            singlePrefabPool = new SinglePrefabPool<TComponent>(component, parent: component.transform.parent);
            InitEvent();
            InitCpntValue();
        }

        private void InitCpntValue()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                bindListFunc(NotifyCollectionChangedAction.Add, _list[i], i);
            }
        }

        private void InitEvent()
        {
            var bindList = _component as IBindList<TVm>;
            bindListFunc = bindList == null ? DefaultBindListFunc : bindList.GetBindListFunc();
            _list.AddListener(bindListFunc);
        }
        
        private void DefaultBindListFunc
            (NotifyCollectionChangedAction type, TVm obj, int index)
        {
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    var gen = singlePrefabPool.Allocate();
                    onCreate?.Invoke(gen, obj);
                    _allObj.Add(gen);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    onDestroy?.Invoke(_allObj[index], obj);
                    singlePrefabPool.Free(_allObj[index]);
                    _allObj.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Log.Warning("default bind list not support replace");
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _allObj.ForEach(com => singlePrefabPool.Free(com));
                    _allObj.Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        protected override void OnReset()
        {
            _list.RemoveListener(bindListFunc);
            singlePrefabPool.Dispose();
        }

        protected override void OnClear()
        {
            _component = default;
            _list = default;
            singlePrefabPool = null;
        }
    }
}