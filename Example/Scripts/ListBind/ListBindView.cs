using System.Collections.Generic;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using Framework.UI.Example;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Example
{
    [UI("ListBind")]
    public class ListBindView : View
    {
        private ListBindViewModel vm;
        private ItemView item;

        [TransformPath("Dropdown")] private Dropdown dropdown;

        [TransformPath("btn_delete")] private Button deleteBtn;

        [TransformPath("btn_add")] private Button addBtn;
        
        [TransformPath("btn_clear")] private Button clearBtn;

        [TransformPath("btn_update")] private Button updateBtn;

        [TransformPath("Scroll View/Viewport/Content")]
        private Transform ItemRoot;

        public override UILevel UILevel { get; } = UILevel.Common;

        protected override void OnVmChange()
        {
            vm = ViewModel as ListBindViewModel;
            dropdown.options = vm.DropdownData;
            Binding.RevertBind(dropdown, vm.SelectedDropDownIndex);
            Binding.BindViewList<ItemViewModel, ItemView>(vm.Items, ItemRoot);
            Binding.BindCommand(addBtn, vm.AddItem);
            Binding.BindCommand(deleteBtn, vm.DeleteSelectedItem);
            Binding.BindCommand(updateBtn, vm.UpdateItem);
            Binding.BindCommand(clearBtn, vm.ClearItem);
        }
    }

    public class ListBindViewModel : ViewModel
    {
        public ObservableList<ItemViewModel> Items { get; }
        public List<Dropdown.OptionData> DropdownData { get; }
        public ObservableProperty<int> SelectedDropDownIndex { get; }

        private ItemViewModel selectedItem;

        public ListBindViewModel()
        {
            Items = new ObservableList<ItemViewModel>() {new ItemViewModel(false, "回锅肉", OnItemClick)};
            SelectedDropDownIndex = new ObservableProperty<int>(0);
            DropdownData = new List<Dropdown.OptionData>()
            {
                new Dropdown.OptionData("回锅肉"),
                new Dropdown.OptionData("水煮肉片"),
                new Dropdown.OptionData("水煮鱼"),
                new Dropdown.OptionData("辣子鸡丁"),
                new Dropdown.OptionData("鱼香肉丝")
            };
            Items.AddListener((list) => OnUpdateItem());
        }

        public void DeleteSelectedItem()
        {
            if (selectedItem == null) return;
            Items.Remove(selectedItem);
            selectedItem = null;
        }

        public void AddItem()
        {
            var vm = CreateItem();
            AddItem(vm);
        }

        public void ClearItem()
        {
            Items.Clear();
        }

        public void UpdateItem()
        {
            selectedItem?.SetPath(DropdownData[SelectedDropDownIndex.Value].text);
        }

        private void AddItem(ItemViewModel itemViewModel)
        {
            Items.Add(itemViewModel);
        }

        private ItemViewModel CreateItem()
        {
            var vm = new ItemViewModel(false, DropdownData[SelectedDropDownIndex.Value].text, OnItemClick);
            return vm;
        }

        private void OnItemClick(ItemViewModel viewModel)
        {
            selectedItem?.OnItemDeselected();
            selectedItem = selectedItem == viewModel ? null : viewModel;
        }

        private void OnUpdateItem()
        {
            if (Items.Count <= 0) return;
            var lastVm = Items[Items.Count - 1];
            foreach (var itemViewModel in Items) itemViewModel.SetLast(itemViewModel == lastVm);
        }
    }
}