using System;
using Framework.UI.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.UI.Example
{
    public class SetupView : View
    {
        public Text nameMessageText;
        public Text mulBindText;
        public InputField atkInputField;
        public Toggle joinToggle;
        public Button joinInButton;
        public Image img;
        public Slider slider;
        public SetupViewModel vm;
        public View subView;
        private UIBindFactory<SetupView, SetupViewModel> binding;

        protected override void OnVmChange ()
        {
            vm = ViewModel as SetupViewModel;
            if(binding == null)
                binding = new UIBindFactory<SetupView, SetupViewModel>(this, vm);
            binding.UpdateVm();
            /*binding.Bind(nameMessageText, viewModel.Visible);
            binding.Bind(nameMessageText, viewModel.Process, process => $"进度为:{process}");
            //binding.Bind(nameMessageText, viewModel.Name,  process => $"进度为:{process}");
            binding.Bind (mulBindText, viewModel.Name, viewModel.ATK,
                          (name, atk) => $"name = {name} atk = {atk.ToString ()}",(str)=>mulBindText.text = $"111{str}");*/
            binding.Bind(joinInButton, vm.OnButtonClick, wrapFunc: click => () =>
            {
                click();
                print("Wrap Button");
            });
            binding.Bind(joinInButton, () => vm.OnInputChanged("a"));
            /*binding.RevertBind(slider, viewModel.Process);
            
            binding.Bind (img, viewModel.Path);
            binding.BindData(viewModel.Visible, viewModel.OnToggleChanged);
            binding.RevertBind(joinToggle, viewModel.Visible);
            binding.RevertBind(atkInputField, viewModel.ATK, (string str) => int.Parse(str));*/
        }

        public override UILevel UILevel { get; } = UILevel.Common;

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }
    }
}