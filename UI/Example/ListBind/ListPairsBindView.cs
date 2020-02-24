using Framework.UI.Core;
using Framework.UI.Example;

public class ListPairsBindView : View
{
    private ListPairsBindViewModel vm;
    public override UILevel UILevel { get; } = UILevel.Common;

    protected override void OnVmChange()
    {
        vm = ViewModel as ListPairsBindViewModel;
        UIBindFactory<ListPairsBindView, ListPairsBindViewModel> binding =
            new UIBindFactory<ListPairsBindView, ListPairsBindViewModel>(this, vm);
        binding.BindIpairs(vm.Items, "item[?]");
    }
}

public class ListPairsBindViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items { get; private set; }

    public ListPairsBindViewModel()
    {
        Items = new BindableList<ItemViewModel>()
        {
            new ItemViewModel(false,"回锅肉",null),
            new ItemViewModel(false,"梅菜扣肉",null),
            new ItemViewModel(false,"水煮鱼",null),
            new ItemViewModel(true,"鱼香肉丝",null),
        };
    }

    public override string ViewPath { get; } = "ListPairsBind";
}

