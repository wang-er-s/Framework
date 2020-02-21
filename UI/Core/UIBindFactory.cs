namespace Framework.UI.Core
{
    public class UIBindFactory<TView, TVm> : BindFactory<TView, TVm> 
        where TView : View
        where TVm : ViewModel
    {
        public BindList<TItemVm> BindList<TItemVm>
            (BindableList<TItemVm> list, params View[] views) where TItemVm : ViewModel
        {
            _canClearListeners.Add(list);
            return new BindList<TItemVm>(list, views);
        }

        public BindIpairsView<TItemVm> BindIpairs<TItemVm>
            (BindableList<TItemVm> list, string pattern) where TItemVm : ViewModel
        {
            _canClearListeners.Add(list);
            return new BindIpairsView<TItemVm>(ref list, pattern, _view.transform);
        }

        public UIBindFactory(TView _view, TVm _vm) : base(_view, _vm)
        {
        }
    }
}