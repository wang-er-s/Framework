using Framework.UI.Core;
using Framework.UI.Example;

public class ListPairsBindView : View
{
    private ListPairsBindViewModel vm;
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
            new ItemViewModel(){Path = new BindableProperty<string>("回锅肉")},
            new ItemViewModel(){Path = new BindableProperty<string>("梅菜扣肉")},
            new ItemViewModel(){Path = new BindableProperty<string>("水煮鱼")},
            new ItemViewModel(){Path = new BindableProperty<string>("鱼香肉丝")},
        };
    }
}

