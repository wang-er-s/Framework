using System;
using System.Collections.Specialized;
using Framework.UI.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class ViewWrapper : BaseWrapper<View>, IBindList<ViewModel>
    {
        private Transform _content;
        private Transform _item;
        private UIManager _uiManager;
        private int _tag;
        private int _index;

        public ViewWrapper (View view, int index = 0) : base (view)
        {
            SetUIManager(view);
            _item = view.transform;
            _content = _item.parent;
            _tag = 0;
            _index = index;
        }

        private void SetUIManager(View view)
        {
            var views = view.GetComponentsInParent<View>();
            foreach (var _view in views)
            {
                if (_view.UiManager == null) continue;
                _uiManager = _view.UiManager;
                break;
            }
        }

        public void SetTag (int tag)
        {
            _tag = tag;
        }

        Action<NotifyCollectionChangedAction, ViewModel, int> IBindList<ViewModel>.GetBindListFunc ()
        {
            return BindListFunc;
        }

        private void BindListFunc
            (NotifyCollectionChangedAction type, ViewModel newViewModel, int index)
        {
            int tag = GetTag (newViewModel);
            if ( tag != _tag ) return;
            switch ( type )
            {
                case NotifyCollectionChangedAction.Add:
                    AddItem (index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveItem (index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceItem (index, newViewModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear ();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException (nameof (type), type, null);
            }
        }
        
        private void AddItem (int index, ViewModel vm)
        {
            _uiManager.CreateListItem (_item, vm, index + 1);
        }

        private void RemoveItem (int index)
        {
            Object.Destroy (_content.GetChild (index + 1).gameObject);
        }

        private void ReplaceItem (int index, ViewModel vm)
        {
            RemoveItem (index);
            AddItem (index, vm);
        }

        private void Clear ()
        {
            int childCount = _content.childCount;
            for ( int i = 1; i < childCount - 1; i++ )
            {
                RemoveItem (i);
            }
        }

        private int GetTag (ViewModel vm)
        {
            var _vm = vm as IBindMulView;
            if ( _vm==null ) return 0;
            return _vm.Tag;
        }
    }
}