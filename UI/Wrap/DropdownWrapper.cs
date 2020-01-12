using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class DropdownWrapper : BaseWrapper<Dropdown> , IFieldChangeCb<int>, IComponentEvent<int>
    {
        private Dropdown dropdown;
        public DropdownWrapper(Dropdown _component) : base(_component)
        {
            dropdown = _component;
        }

        public Action<int> GetFieldChangeCb()
        {
            return (index) => dropdown.value = index;
        }

        public UnityEvent<int> GetComponentEvent()
        {
            return dropdown.onValueChanged;
        }
    }
}