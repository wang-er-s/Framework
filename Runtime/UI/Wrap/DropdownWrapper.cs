using System;
using System.Collections.Specialized;
using Framework.UI.Wrap.Base;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class DropdownWrapper : BaseWrapper<Dropdown>, IFieldChangeCb<int>, IComponentEvent<int>,
        IBindList<Dropdown.OptionData>
    {
        public DropdownWrapper(Dropdown view) : base(view)
        {
            this.View = view;
        }

        public Action<int> GetFieldChangeCb()
        {
            return (index) => View.value = index;
        }

        public UnityEvent<int> GetComponentEvent()
        {
            return View.onValueChanged;
        }

        public Action<NotifyCollectionChangedAction, Dropdown.OptionData, int> GetBindListFunc()
        {
            return BindListFunc;
        }
        
        private void BindListFunc
            (NotifyCollectionChangedAction type, Dropdown.OptionData data, int index)
        {
            switch (type)
            {
                case NotifyCollectionChangedAction.Add:
                    View.options.Insert(index, data);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    View.options.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    View.options[index] = data;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void Clear()
        {
            View.options.Clear();
        }
    }
}