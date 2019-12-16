using System.Collections;
using System.Collections.Generic;
using AD;
using AD.UI.Core;
using AD.UI.Example;
using UnityEngine;
using UnityEngine.UI;

public class ListBindView : View
{
    
    public ItemView item;
    public Dropdown dropdown;
    public Button deleteBtn;
    public Button addBtn;
    public Button updateBtn;
    private ListBindViewModel viewModel;

    protected override void OnVmChange()
    {
        viewModel = VM as ListBindViewModel;
        BindFactory<ListBindView, ListBindViewModel> binding =
            new BindFactory<ListBindView, ListBindViewModel>(this, viewModel);
        dropdown.options = viewModel.DropdownData;
        binding.RevertBind(dropdown, viewModel.SelectedDropDownIndex).InitBind();
        binding.BindList(viewModel.Items, item).InitBind();
        binding.BindCommand(addBtn,viewModel.AddItem).InitBind();
        // 删除释放绑定？
        //binding.BindCommand(deleteBtn, viewModel.DeleteSelectedItem).InitBind();
        binding.BindCommand(updateBtn, viewModel.UpdateItem).InitBind();

    }
}

public class ListBindViewModel : ViewModel
{
    public BindableList<ItemViewModel> Items { get; private set; }
    public List<Dropdown.OptionData> DropdownData { get; private set; }
    public BindableProperty<int> SelectedDropDownIndex { get; private set; }
    public BindableProperty<int> SelectedItemIndex { get; private set; }

    private EventService EventService;

    public ListBindViewModel()
    {
        Items = new BindableList<ItemViewModel>();
        SelectedDropDownIndex = new BindableProperty<int>(0);
        SelectedItemIndex = new BindableProperty<int>(-1);
        DropdownData = new List<Dropdown.OptionData>()
        {
            new Dropdown.OptionData("回锅肉"),
            new Dropdown.OptionData("梅菜扣肉"),
            new Dropdown.OptionData("水煮肉片"),
            new Dropdown.OptionData("辣子鸡丁"),
            new Dropdown.OptionData("鱼香肉丝"),
        };
    }

    public void DeleteSelectedItem()
    {
        if(SelectedItemIndex.Value == -1) return;
        Items.RemoveAt(SelectedItemIndex.Value);
    }

    public void AddItem()
    {
        var vm = CreateItem();
        AddItem(vm);
    }

    public void UpdateItem()
    {
        if(SelectedItemIndex.Value == -1) return;
        var item = Items[SelectedItemIndex.Value];
        item.Path.Value = DropdownData[SelectedDropDownIndex.Value].text;
    }
    
    private void AddItem(ItemViewModel itemViewModel)
    {
        Items.AddListUpdateListener((list)=>itemViewModel.Last.Value = itemViewModel.Index.Value == list.Count - 1);
        SelectedItemIndex.AddChangeEvent((index)=>itemViewModel.Selected.Value = itemViewModel.Index.Value == index);
        Items.Add(itemViewModel);
    }

    private ItemViewModel CreateItem()
    {
        ItemViewModel vm = new ItemViewModel
        {
            Index = new BindableProperty<int>(Items.Count),
            Last = new BindableProperty<bool>(false),
            Path = new BindableProperty<string>(DropdownData[SelectedDropDownIndex.Value].text),
        };
        vm.OnItemClick = () => OnItemClick(vm.Index.Value);
        return vm;
    }

    private void OnItemClick(int index)
    {
        SelectedItemIndex.Value = index;
    }
    
}
