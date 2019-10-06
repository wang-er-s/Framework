using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nine.UI.Core;
using UnityEngine;

namespace Assets.Nine.UI.Wrap
{
    class ViewWrapper : BaseWrapper<View<ViewModel>>, IBindList<ViewModel>
    {

        private Transform content;
        private Transform item;
        private View<ViewModel> view;

        public ViewWrapper(View<ViewModel> _view) : base(_view)
        {
            view = _view;
            item = view.transform;
            content = item.parent;
        }

        public Action<NotifyCollectionChangedAction, ViewModel, ViewModel, int> GetBindListFunc()
        {
            return BindListFunc;
        }

        private void BindListFunc(NotifyCollectionChangedAction type, ViewModel oldViewModel, ViewModel newViewModel,
            int index)
        {
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    break;
                case NotifyCollectionChangedAction.Remove:
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }
    }
}
