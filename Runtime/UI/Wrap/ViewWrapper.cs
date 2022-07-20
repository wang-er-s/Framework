using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Framework.Assets;
using Framework.Asynchronous;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Wrap.Base;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class ViewWrapper: BaseWrapper<View>, IBindList<ViewModel>
    {
        private readonly Transform _content;
        private readonly View _item;
        private readonly GameObject _template;
        private List<View> existViews = new List<View>();
        private int _tag;
        private int _index;

        public ViewWrapper(View component, Transform root, int index = 0) : base(component, component)
        {
            _item = component;
            _content = root;
            Log.Assert(_content.childCount == 1 , "_content.childCount 只能有一个");
            _template = _content.GetChild(0).gameObject;
            _template.ActiveHide();
            _tag = 0;
            _index = index;
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
                    Clear(index);
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void AddItem(int index, ViewModel vm)
        {
            var view = ReflectionHelper.CreateInstance(_item.GetCLRType()) as View;
            var go = Object.Instantiate(_template, _content);
            go.transform.SetSiblingIndex(index + 1);
            go.ActiveShow();
            view.SetGameObject(go);
            view.SetVm(vm);
            view.Show();
            existViews.Insert(index, view);
        }

        private void RemoveItem(int index)
        {
            Object.DestroyImmediate(_content.GetChild(index+1).gameObject);
            existViews.RemoveAt(index);
        }

        private void ReplaceItem(int index, ViewModel vm)
        {
            existViews[index].SetVm(vm);
        }

        private void Clear(int itemCount)
        {
            while (itemCount > 0)
            {
                RemoveItem(--itemCount);
            }
            existViews.Clear();
        }

        private static int GetTag(ViewModel vm)
        {
            var _vm = vm as IBindMulView;
            if (_vm == null) return 0;
            return _vm.Tag;
        }
    }
}