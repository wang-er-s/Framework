using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class UIBindFactory : BindFactory
    {
        public void BindViewList<TItemVm, TItemView>
            (ObservableList<TItemVm> list,Transform root) where TItemVm : ViewModel
            where TItemView : Window
        {
            BindViewList<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindViewList<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = ReferencePool.Allocate<BindViewList<TItemVm, TItemView>>();
                bind.Init(Container);
            }
            bind.Reset(list,Container as Window, root);
            AddClearable(bind);
        }
        
        public void BindViewList<TItemVm, TItemView>
            (ObservableList<TItemVm> list,LoopListView2 root, GameObject template) where TItemVm : ViewModel
            where TItemView : LoopListViewItem2, new()
        {
            BindLoopListView<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindLoopListView<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = ReferencePool.Allocate<BindLoopListView<TItemVm, TItemView>>();
                bind.Init(Container);
            }
            bind.Reset(list, root, template);
            AddClearable(bind);
        }
        
        public void BindViewList<TItemVm, TItemView>
            (ObservableList<TItemVm> list,LoopGridView root, GameObject template) where TItemVm : ViewModel
            where TItemView : LoopGridViewItem, new()
        {
            BindLoopGridView<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindLoopGridView<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = ReferencePool.Allocate<BindLoopGridView<TItemVm, TItemView>>();
                bind.Init(Container);
            }
            bind.Reset(list, root, template);
            AddClearable(bind);
        }

        public void BindIpairs<TItemVm, TItemView>
            (ObservableList<TItemVm> list, Transform root, string pattern) where TItemVm : ViewModel
            where TItemView : Window
        {
            BindIpairsViewList<TItemVm, TItemView> bind;
            if (CacheBinds.Count > 0)
            {
                bind = (BindIpairsViewList<TItemVm, TItemView>) CacheBinds.Dequeue();
            }
            else
            {
                bind = ReferencePool.Allocate<BindIpairsViewList<TItemVm, TItemView>>();
                bind.Init(Container);
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
    }
}