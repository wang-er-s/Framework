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
        
        /// <summary>
        /// 用在热更的BindView
        /// </summary>
        public void BindViewList(ObservableList<ViewModelAdapter.Adapter> list,Transform root, Type view)
        {
            var bind = new BindViewList<ViewModelAdapter.Adapter>(list, root, view);
            Binds.Add(bind);
        }

        public void BindIpairs<TItemVm, TItemView>
            (ObservableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
        where TItemView : View
        {
            var bind = new BindIpairsViewList<TItemVm, TItemView>(list, pattern, root);
            Binds.Add(bind);
        }
        
        public void BindIpairs<TItemVm>
            (ObservableList<TItemVm> list, Transform root, string pattern, Type view) where TItemVm : ViewModel
        {
            var bind = new BindIpairsViewList<TItemVm>(list, pattern, root, view);
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