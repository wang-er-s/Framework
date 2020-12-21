using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Core.Bind
{
    public class UIBindFactory : BindFactory
    {
        public void BindViewList<TItemVm, TItemView>
            (ObservableList<TItemVm> list,Transform root) where TItemVm : ViewModel
        where TItemView : View
        {
            var bind = new BindViewList<TItemVm, TItemView>(list,root);
            Binds.Add(bind);
        }

        public void BindIpairs<TItemVm, TItemView>
            (ObservableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
        where TItemView : View
        {
            var bind = new BindIpairsViewList<TItemVm, TItemView>(list, pattern, root);
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