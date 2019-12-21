/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AD.UI.Core;
using AD;
using AD.UI.Example;
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
            BindFactory<ItemView, ItemViewModel> binding = new BindFactory<ItemView, ItemViewModel>(this, this.viewModel);
            binding.Bind(itemImg, viewModel.Path).InitBind();
            binding.Bind(nameTxt, viewModel.Path).InitBind();
            binding.BindData(viewModel.Last, (last) => breakLine.SetActive(!last));
            binding.BindData(viewModel.Selected, (value) => selected.SetActive(value));
            if (viewModel.OnItemClick != null)
                binding.BindCommand(selfBtn, viewModel.OnItemClick).InitBind();
        }

    }
    
    public class ItemViewModel : ViewModel
    {
        public IBindableField<string> Path { get; set; }
        //如果是最后一个item，则要隐藏breakline
        public IBindableField<bool> Last { get; set; }
        public IBindableField<int> Index { get; set; }
        public IBindableField<bool> Selected { get; set; }
        public Action OnItemClick { get; set; }

        public ItemViewModel()
        {
            Path = new BindableField<string>();
            Last = new BindableField<bool>();
            Index = new BindableField<int>();
            Selected = new BindableField<bool>();
        }

    }
}
*/
