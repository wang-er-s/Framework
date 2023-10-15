using System;
using System.Collections.Specialized;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework
{
    public class DropdownWrapper : BaseWrapper<Dropdown>, IFieldChangeCb<int>, IComponentEvent<int>,
        IBindList<Dropdown.OptionData>
    {

        public Action<int> GetFieldChangeCb()
        {
            return (index) => Component.value = index;
        }

        public UnityEvent<int> GetComponentEvent()
        {
            return Component.onValueChanged;
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
                    Component.options.Insert(index, data);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Component.options.RemoveAt(index);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Component.options[index] = data;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Component.options.Clear();
                    break;
                case NotifyCollectionChangedAction.Move: break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}