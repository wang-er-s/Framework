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
            AddClearable(bind);
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
            AddClearable(bind);
        }
        
#if ILRUNTIME
        /// <summary>
        /// 用在热更的BindView
        /// </summary>
        public void BindViewList(ObservableList<ViewModelAdapter.Adapter> list,LoopScrollRect root, Type view)
        {
            BindLoopViewList<ViewModelAdapter.Adapter,ViewAdapter.Adapter> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindLoopViewList<ViewModelAdapter.Adapter,ViewAdapter.Adapter>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindLoopViewList<ViewModelAdapter.Adapter,ViewAdapter.Adapter>();
            }
            bind.SetViewType(view);
            bind.Reset(list, root);
            AddClearable(bind);
        }
        
        /// <summary>
        /// 用在热更的BindView
        /// </summary>
        public void BindViewList(ObservableList<ViewModelAdapter.Adapter> list,Transform root, Type view)
        {
            BindViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter>();
            }
            bind.SetViewType(view);
            bind.Reset(list, root);
            AddClearable(bind);
        }
        
        public void BindIpairs
            (ObservableList<ViewModelAdapter.Adapter> list, Transform root, string pattern, Type view)
        {
            BindIpairsViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindIpairsViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter>) CacheBinds.Dequeue();
            }
            else
            {
                bind = new BindIpairsViewList<ViewModelAdapter.Adapter, ViewAdapter.Adapter>();
            }
            bind.SetViewType(view);
            bind.Reset(list, pattern, root);
            AddClearable(bind);
        }

#endif
        
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
            AddClearable(bind);
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