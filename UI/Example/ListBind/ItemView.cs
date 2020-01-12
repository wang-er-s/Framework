using System;
using AD.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace AD.UI.Example
{

    public class ItemView : View
    {
        public Image itemImg;
        public Text nameTxt;
        public Button selfBtn;
        public GameObject breakLine;
        public GameObject selected;
        private ItemViewModel viewModel;

        protected override void OnVmChange()
        {
            viewModel = VM as ItemViewModel;
            UIBindFactory<ItemView, ItemViewModel> binding = new UIBindFactory<ItemView, ItemViewModel>(this, this.viewModel);
            binding.Bind(itemImg, viewModel.Path);
            binding.Bind(nameTxt, viewModel.Path);
            binding.BindData(viewModel.Last, (last) => breakLine.SetActive(!last));
            binding.BindData(viewModel.Selected, (value) => selected.SetActive(value));
            if (viewModel.OnItemClick != null)
                binding.Bind(selfBtn, viewModel.OnItemClick);
        }

    }
    
    public class ItemViewModel : ViewModel
    {
        public IBindableProperty<string> Path { get; set; }
        //如果是最后一个item，则要隐藏breakline
        public IBindableProperty<bool> Last { get; set; }
        public IBindableProperty<int> Index { get; set; }
        public IBindableProperty<bool> Selected { get; set; }
        public Action OnItemClick { get; set; }

        public ItemViewModel()
        {
            Path = new BindableProperty<string>();
            Last = new BindableProperty<bool>();
            Index = new BindableProperty<int>();
            Selected = new BindableProperty<bool>();
        }

    }
}

