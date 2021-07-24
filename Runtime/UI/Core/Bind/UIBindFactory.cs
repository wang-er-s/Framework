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
            BindViewList<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindViewList<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindViewList<TItemVm, TItemView>();
            }
            bind.Reset(list, root);
            clearables.Add(bind);
        }
        
        public void BindViewList<TItemVm, TItemView>
            (ObservableList<TItemVm> list,LoopScrollRect root) where TItemVm : ViewModel
            where TItemView : View , new()
        {
            BindLoopViewList<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindLoopViewList<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindLoopViewList<TItemVm, TItemView>();
            }
            bind.Reset(list, root);
            clearables.Add(bind);
        }
        
        /// <summary>
        /// 用在热更的BindView
        /// </summary>
        public void BindViewList(ObservableList<ViewModelAdapter.Adapter> list,Transform root, Type view)
        {
            BindViewList<ViewModelAdapter.Adapter> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindViewList<ViewModelAdapter.Adapter>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindViewList<ViewModelAdapter.Adapter>();
            }
            bind.Reset(list, root, view);
            clearables.Add(bind);
        }

        public void BindIpairs<TItemVm, TItemView>
            (ObservableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
        where TItemView : View
        {
            BindIpairsViewList<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindIpairsViewList<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindIpairsViewList<TItemVm, TItemView>();
            }
            bind.Reset(list, pattern, root);
            clearables.Add(bind);
        }
        
        public void BindIpairs<TItemVm>
            (ObservableList<TItemVm> list, Transform root, string pattern, Type view) where TItemVm : ViewModel
        {
            BindIpairsViewList<TItemVm> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindIpairsViewList<TItemVm>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindIpairsViewList<TItemVm>();
            }
            bind.Reset(list, pattern, root, view);
            clearables.Add(bind);
        }

        public void BindDropDown(Dropdown dropdown, ObservableProperty<int> property,
            ObservableList<Dropdown.OptionData> listProperty = null)
        {
            TwoWayBind(dropdown, property);
            BindList(dropdown, listProperty);
        }

        public UIBindFactory(View view) : base(view)
        {
        }
    }
}