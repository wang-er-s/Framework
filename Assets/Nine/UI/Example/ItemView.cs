using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nine.UI.Core;
using Nine.UI.Core;
using Nine.UI.Example;
using UnityEngine.UI;

namespace Assets.Nine.UI.Example
{

    public class ItemView : View
    {
        public Image img;
        public ItemViewModel viewModel;

        protected override void OnCreate(ViewModel viewModel)
        {
            this.viewModel = viewModel as ItemViewModel;
            BindFactory<ItemView, ItemViewModel> binding = new BindFactory<ItemView, ItemViewModel>(this, this.viewModel);
            binding.Bind(img, (vm) => vm.Path).OneWay();
        }
    }
}
