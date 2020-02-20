using System.Collections.Generic;
using Framework.UI.Core;
using Framework.UI.Example;
using UnityEngine.UI;

public class ListBindView : View
{
    private UIBindFactory<ListBindView, ListBindViewModel> binding;
    private ListBindViewModel vm;
    public ItemView item;
    public Dropdown dropdown;
    public Button deleteBtn;
    public Button addBtn;
    public Button updateBtn;

    public override UILevel UILevel { get; } = UILevel.Common;

    protected override void OnVmChange()
    {
        vm = ViewModel as ListBindViewModel;
        if (binding == null)
            binding = new UIBindFactory<ListBindView, ListBindViewModel>(this, vm);
        dropdown.options = vm.DropdownData;
        binding.RevertBind(dropdown, vm.SelectedDropDownIndex);
        binding.BindList(vm.Items, item);
        binding.Bind(addBtn, vm.AddItem);
        binding.Bind(deleteBtn, vm.DeleteSelectedItem);
        binding.Bind(updateBtn, vm.UpdateItem);

    }
}

public class ListBindViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items { get; private set; }
    public List<Dropdown.OptionData> DropdownData { get; private set; }
    public BindableProperty<int> SelectedDropDownIndex { get; private set; }
    private int selectedDropDownIndex
    {
        get { return SelectedDropDownIndex; }
        set { ((IBindableProperty<int>) SelectedDropDownIndex).Value = value; }
    }
    private ItemViewModel selectedItem;
    
    public ListBindViewModel()
    {
        Items = new BindableList<ItemViewModel>();
        SelectedDropDownIndex = new BindableProperty<int>(0);
        DropdownData = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("回锅肉"),
            new Dropdown.OptionData("梅菜扣肉"),
            new Dropdown.OptionData("水煮肉片"),
            new Dropdown.OptionData("辣子鸡丁"),
            new Dropdown.OptionData("鱼香肉丝"),
        };
        Items.AddListUpdateListener((list) => OnUpdateItem());
    }

    public void DeleteSelectedItem()
    {
        if(selectedItem == null) return;
        Items.Remove(selectedItem);
        selectedItem = null;
    }

    public void AddItem()
    {
        var vm = CreateItem();
        AddItem(vm);
    }

    public void UpdateItem()
    {
        if(selectedItem == null) return;
        selectedItem.Path.Value = DropdownData[selectedDropDownIndex].text;
    }
    
    private void AddItem(ItemViewModel itemViewModel)
    {
        Items.Add(itemViewModel);
    }

    private ItemViewModel CreateItem()
    {
        ItemViewModel vm = new ItemViewModel
        {
            Last = new BindableProperty<bool>(false),
            Path = new BindableProperty<string>(DropdownData[selectedDropDownIndex].text),
        };
        vm.ItemClickCb = OnItemClick;
        return vm;
    }

    private void OnItemClick(ItemViewModel viewModel)
    {
        selectedItem?.OnItemDeselected();
        selectedItem = viewModel;
    }

    private void OnUpdateItem()
    {
        var lastVm = Items[Items.Count - 1];
        foreach (var itemViewModel in Items)
        {
            itemViewModel.Last.Value = itemViewModel == lastVm;
        }
    }

    public override string ViewPath { get; } = "";
}

