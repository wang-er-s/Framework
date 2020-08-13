using System;
using System.Collections.Specialized;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Wrap.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class ViewWrapper : BaseWrapper<View>, IBindList<ViewModel>
    {
        private readonly Transform _content;
        private readonly Transform _item;
        private int _tag;
        private int _index;

        public ViewWrapper(View view, int index = 0) : base(view)
        {
            _item = view.transform;
            _content = _item.parent;
            _tag = 0;
            this._index = index;
        }

        public void SetTag(int tag)
        {
            this._tag = tag;
        }

        Action<NotifyCollectionChangedAction, ViewModel, int> IBindList<ViewModel>.GetBindListFunc()
        {
            return BindListFunc;
        }

        private void BindListFunc
            (NotifyCollectionChangedAction type, ViewModel newViewModel, int index)
        {
            var tag = GetTag(newViewModel);
            if (tag != this._tag) return;
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    AddItem(index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItem(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItem(index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void AddItem(int index, ViewModel vm)
        {
            var go = Object.Instantiate(_item, _content);
            var view = go.GetComponent<View>();
            view.SetVm(vm);
            go.transform.SetSiblingIndex(index + 1);
            view.Show();
        }

        private void RemoveItem(int index)
        {
            Object.Destroy(_content.GetChild(index + 1).gameObject);
        }

        private void ReplaceItem(int index, ViewModel vm)
        {
            RemoveItem(index);
            AddItem(index, vm);
        }

        private void Clear()
        {
            while (_content.childCount > 1)
            {
                RemoveItem(1);
            }
        }

        private static int GetTag(ViewModel vm)
        {
            var _vm = vm as IBindMulView;
            if (_vm == null) return 0;
            return _vm.Tag;
        }
    }
}