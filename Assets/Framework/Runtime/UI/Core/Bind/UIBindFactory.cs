using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Core.Bind
{
    public class UIBindFactory : BindFactory
    {
        public void BindViewList<TItemVm>
            (ObservableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            var bind = new BindViewList<TItemVm>(list, views);
            Binds.Add(bind);
        }

        public void BindIpairs<TItemVm>
            (ObservableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
        {
            var bind = new BindIpairsView<TItemVm>(list, pattern, root);
            Binds.Add(bind);
        }

        public void BindDropDown(Dropdown dropdown, ObservableProperty<int> property,
            ObservableList<Dropdown.OptionData> listProperty = null)
        {
            TwoWayBind(dropdown, property);
            BindList(dropdown, listProperty);
        }
    }
}