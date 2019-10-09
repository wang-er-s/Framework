using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nine.UI.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Nine.UI.Wrap
{
    public class ViewWrapper : BaseWrapper<View>,IBindList<ViewModel>
    {

        private Transform content;
        private Transform item;
        private View view;

        public ViewWrapper(View _view) : base(_view)
        {
            view = _view;
            item = view.transform;
            content = item.parent;
        }

        Action< NotifyCollectionChangedAction, ViewModel, ViewModel, int> IBindList<ViewModel>.GetBindListFunc()
        {
            return BindListFunc;
        }

        private void BindListFunc(NotifyCollectionChangedAction type, ViewModel oldViewModel, ViewModel newViewModel,
            int index)
        {
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
            }
        }

        private void AddItem(int index, ViewModel vm)
        {
            UIMgr.Ins.CreateListItem(item, vm, index + 1);
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
            int childCount = content.childCount;
            for (int i = 0; i < childCount - 1; i++)
            {
                RemoveItem(i);
            }
        }

    }
}
