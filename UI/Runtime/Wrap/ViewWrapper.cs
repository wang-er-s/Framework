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
        private Transform content;
        private Transform item;
        private int tag;
        private int index;

        public ViewWrapper(View view, int index = 0) : base(view)
        {
            item = view.transform;
            content = item.parent;
            tag = 0;
            this.index = index;
        }

        public void SetTag(int tag)
        {
            this.tag = tag;
        }

        Action<NotifyCollectionChangedAction, ViewModel, int> IBindList<ViewModel>.GetBindListFunc()
        {
            return BindListFunc;
        }

        private void BindListFunc
            (NotifyCollectionChangedAction type, ViewModel newViewModel, int index)
        {
            var _tag = GetTag(newViewModel);
            if (_tag != tag) return;
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
            var go = Object.Instantiate(item, content);
            var _view = go.GetComponent<View>();
            _view.SetVM(vm);
            go.transform.SetSiblingIndex(index + 1);
            _view.Show();
        }

        private void RemoveItem(int index)
        {
            Object.Destroy(content.GetChild(index + 1).gameObject);
        }

        private void ReplaceItem(int index, ViewModel vm)
        {
            RemoveItem(index);
            AddItem(index, vm);
        }

        private void Clear()
        {
            while (content.childCount > 1)
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