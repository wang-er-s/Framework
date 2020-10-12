using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
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
            Binding.Bind(itemImg, vm.Path);
            Binding.Bind(nameTxt, vm.Path);
            Binding.BindData(vm.Last, (last) => breakLine.SetActive(!last));
            //Binding.BindData(vm.Selected, (value) => selected.SetActive(value));
            Binding.BindData(vm.Selected, CC);
            Binding.BindCommand(selfBtn, vm.OnItemClick);
        }

        public void CC(bool val)
        {
            selected.SetActive(val);
        }
    }


    public class ItemViewModel : ViewModel
    {
        public ObservableProperty<string> Path { get; }
        private string _path
        {
            get => Path;
            set => Path.Value = value;
        }
        //如果是最后一个item，则要隐藏breakline
        public ObservableProperty<bool> Last { get; }
        private bool _last
        {
            get => Last;
            set => Last.Value = value;
        }
        public ObservableProperty<int> Index { get; }
        private int _index
        {
            get => Index;
            set => Index.Value = value;
        }
        public ObservableProperty<bool> Selected { get; }
        private bool _selected
        {
            get => Selected;
            set => Selected.Value = value;
        }
        private Action<ItemViewModel> _itemClickCb;

        public ItemViewModel()
        {
            Path = new ObservableProperty<string>();
            Last = new ObservableProperty<bool>();
            Index = new ObservableProperty<int>();
            Selected = new ObservableProperty<bool>();
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
    }
}