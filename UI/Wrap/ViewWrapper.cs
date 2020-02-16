using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.UI.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.UI.Wrap
{
    public class ViewWrapper : BaseWrapper<View>, IBindList<ViewModel>
    {
        private Transform content;
        private Transform item;
        private UIManager uiManager;
        private int tag;
        private int index;

        public ViewWrapper (View _view, int _index = 0) : base (_view)
        {
            SetUIManager(_view);
            item = _view.transform;
            content = item.parent;
            tag = 0;
            index = _index;
        }

        private void SetUIManager(View view)
        {
            var views = view.GetComponentsInParent<View>();
            foreach (var _view in views)
            {
                if (_view.UiManager == null) continue;
                uiManager = _view.UiManager;
                break;
            }
        }

        public void SetTag (int _tag)
        {
            tag = _tag;
        }

        Action<NotifyCollectionChangedAction, ViewModel, ViewModel, int> IBindList<ViewModel>.GetBindListFunc ()
        {
            return BindListFunc;
        }

        private void BindListFunc
            (NotifyCollectionChangedAction type, ViewModel oldViewModel, ViewModel newViewModel, int index)
        {
            int _tag = GetTag (newViewModel);
            if ( _tag != tag ) return;
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
            uiManager.CreateListItem (item, vm, index + 1);
        }

        private void RemoveItem (int index)
        {
            Object.Destroy (content.GetChild (index + 1).gameObject);
        }

        private void ReplaceItem (int index, ViewModel vm)
        {
            RemoveItem (index);
            AddItem (index, vm);
        }

        private void Clear ()
        {
            int childCount = content.childCount;
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