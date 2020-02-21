using System;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class DropdownWrapper : BaseWrapper<Dropdown> , IFieldChangeCb<int>, IComponentEvent<int>
    {
        public DropdownWrapper(Dropdown view) : base(view)
        {
            _view = view;
        }

        public Action<int> GetFieldChangeCb()
        {
            return (index) => _view.value = index;
        }

        public UnityEvent<int> GetComponentEvent()
        {
            return _view.onValueChanged;
        }
    }
}