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
            this.view = view;
        }

        public Action<int> GetFieldChangeCb()
        {
            return (index) => view.value = index;
        }

        public UnityEvent<int> GetComponentEvent()
        {
            return view.onValueChanged;
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
                    view.options.Insert(index, data);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    view.options.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    view.options[index] = data;
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
            view.options.Clear();
        }
    }
}