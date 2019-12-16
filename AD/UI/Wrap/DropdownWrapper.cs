using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Wrap
{
    public class DropdownWrapper : BaseWrapper<Dropdown> , IBindData<int>, IBindCommand<int>
    {
        private Dropdown dropdown;
        public DropdownWrapper(Dropdown _component) : base(_component)
        {
            dropdown = _component;
        }

        public Action<int> GetBindFieldFunc()
        {
            return (index) => dropdown.value = index;
        }

        public UnityEvent<int> GetBindCommandFunc()
        {
            return dropdown.onValueChanged;
        }
    }
}