using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Nine.UI.Core;
using Nine;
using Nine.UI.Core;
using Nine.UI.Example;
using UnityEngine.UI;

namespace Assets.Nine.UI.Example
{

    public class ItemView : View
    {
        public Image img;
        public Text text;
        public ItemViewModel viewModel;

        protected override void OnCreate()
        {
            viewModel = data as ItemViewModel;
            BindFactory<ItemView, ItemViewModel> binding = new BindFactory<ItemView, ItemViewModel>(this, this.viewModel);
            binding.Bind(img, (vm) => vm.Path).OneWay();
        }

        protected override ViewModel CreateVM ()
        {
            return new ItemViewModel();
        }
    }
}
