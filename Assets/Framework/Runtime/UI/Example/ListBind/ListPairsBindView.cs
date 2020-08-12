using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;

public class ListPairsBindView : View
{
    private ListPairsBindViewModel vm;
    public override UILevel UILevel { get; } = UILevel.Common;

    public Transform ItemRoot;

    protected override void OnVmChange()
    {
        vm = ViewModel as ListPairsBindViewModel;
        var binding =
            new UIBindFactory<ListPairsBindView, ListPairsBindViewModel>(this, vm);
        binding.BindIpairs(vm.Items, ItemRoot, "item[?]");
    }
}

public class ListPairsBindViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items { get; private set; }

    public ListPairsBindViewModel()
    {
        Items = new BindableList<ItemViewModel>()
        {
            new ItemViewModel(false, "回锅肉", null),
            new ItemViewModel(false, "梅菜扣肉", null),
            new ItemViewModel(false, "水煮鱼", null),
            new ItemViewModel(true, "鱼香肉丝", null)
        };
    }

    public override string ViewPath { get; } = "ListPairsBind";

    public static ListPairsBindViewModel Create(VMCreator vmCreator)
    {
        var vm = new ListPairsBindViewModel();
        vmCreator?.BindView(vm);
        return vm;
    }
}