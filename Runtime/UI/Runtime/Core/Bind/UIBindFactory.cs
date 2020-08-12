using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Core.Bind
{
    public class UIBindFactory<TView, TVm> : BindFactory<TView, TVm>
    {
        public void BindViewList<TItemVm>
            (BindableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            canClearListeners.Add(list);
            var bindList = new BindViewList<TItemVm>(list, views);
        }
        
        public void BindIpairs<TItemVm>
            (BindableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
        {
            canClearListeners.Add(list);
            var bindIpairsView = new BindIpairsView<TItemVm>(list, pattern, root);
        }

        public void BindDropDown(Dropdown dropdown, ObservableProperty<int> property,
            BindableList<Dropdown.OptionData> listProperty = null)
        {
            TwoWayBind(dropdown,property);
            BindList(dropdown, listProperty);
        }

        public UIBindFactory(TView view, TVm vm) : base(view, vm)
        {
        }
    }
}