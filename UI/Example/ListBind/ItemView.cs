using System;
using Framework.UI.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Example
{

    public class ItemView : View
    {
        public Image itemImg;
        public Text nameTxt;
        public Button selfBtn;
        public GameObject breakLine;
        public GameObject selected;
        private ItemViewModel vm;

        public override UILevel UILevel { get; } = UILevel.Common;

        protected override void OnVmChange()
        {
            vm = ViewModel as ItemViewModel;
            UIBindFactory<ItemView, ItemViewModel> binding = new UIBindFactory<ItemView, ItemViewModel>(this, this.vm);
            binding.Bind(itemImg, vm.Path);
            binding.Bind(nameTxt, vm.Path);
            binding.BindData(vm.Last, (last) => breakLine.SetActive(!last));
            //binding.BindData(vm.Selected, (value) => selected.SetActive(value));
            binding.BindData(vm.Selected, CC);
            binding.BindCommand(selfBtn, vm.OnItemClick);
        }
        
        public void CC(bool val)
        {
            selected.SetActive(val);
        }
    }
    
   

    public class ItemViewModel : ViewModel
    {
        public BindableProperty<string> Path { get; }
        private string _path
        {
            get => Path;
            set => ((IBindableProperty<string>) Path).Value = value;
        }
        //如果是最后一个item，则要隐藏breakline
        public BindableProperty<bool> Last { get; }
        private bool _last
        {
            get => Last;
            set => ((IBindableProperty<bool>) Last).Value = value;
        }
        public BindableProperty<int> Index { get; }
        private int _index
        {
            get => Index;
            set => ((IBindableProperty<int>) Index).Value = value;
        }
        public BindableProperty<bool> Selected { get; }
        private bool _selected
        {
            get => Selected;
            set => ((IBindableProperty<bool>) Selected).Value = value;
        }
        private Action<ItemViewModel> _itemClickCb; 

        public ItemViewModel()
        {
            Path = new BindableProperty<string>();
            Last = new BindableProperty<bool>();
            Index = new BindableProperty<int>();
            Selected = new BindableProperty<bool>();
        }

        public ItemViewModel(bool last, string path, Action<ItemViewModel> clickCb) : this()
        {
            _last = last;
            _path = path;
            _itemClickCb = clickCb;
        }

        public void OnItemClick()
        {
            _selected = true;
            _itemClickCb?.Invoke(this);
        }

        public void OnItemDeselected()
        {
            _selected = false;
        }

        public void SetPath(string path)
        {
            _path = path;
        }

        public void SetLast(bool last)
        {
            _last = last;
        }
        
        public override string ViewPath { get; } = "";
    }
}

