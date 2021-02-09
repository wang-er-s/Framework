using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;

namespace Framework.UI.Example
{
    [UI("ListPairsBind")]
    public class ListPairsBindView : View
    {
        private ListPairsBindViewModel vm;
        public override UILevel UILevel { get; } = UILevel.Common;

        [TransformPath("Scroll View/Viewport/Content")]
        private Transform ItemRoot;

        protected override void OnVmChange()
        {
            vm = ViewModel as ListPairsBindViewModel;
            Binding.BindIpairs<ItemViewModel, ItemView>(vm.Items, ItemRoot, "item[?]");
        }
    }

    public class ListPairsBindViewModel : ViewModel
    {
        public ObservableList<ItemViewModel> Items { get; private set; }

        public ListPairsBindViewModel()
        {
            Items = new ObservableList<ItemViewModel>()
            {
                new ItemViewModel(false, "回锅肉", null),
                new ItemViewModel(false, "辣子鸡丁", null),
                new ItemViewModel(false, "水煮肉片", null),
                new ItemViewModel(true, "水煮鱼", null)
            };
        }
    }
}