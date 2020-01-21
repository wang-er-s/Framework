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
        public SetupViewModel viewModel;
        public View subView;
        private UIBindFactory<SetupView, SetupViewModel> binding;

        protected override void OnVmChange ()
        {
            viewModel = ViewModel as SetupViewModel;
            if(binding == null)
                binding = new UIBindFactory<SetupView, SetupViewModel>(this, viewModel);
            binding.UpdateVm();
            /*binding.Bind(nameMessageText, viewModel.Visible);
            binding.Bind(nameMessageText, viewModel.Process, process => $"进度为:{process}");
            //binding.Bind(nameMessageText, viewModel.Name,  process => $"进度为:{process}");
            binding.Bind (mulBindText, viewModel.Name, viewModel.ATK,
                          (name, atk) => $"name = {name} atk = {atk.ToString ()}",(str)=>mulBindText.text = $"111{str}");*/
            binding.Bind(joinInButton, viewModel.OnButtonClick, wrapFunc: click => () =>
            {
                click();
                print("Wrap Button");
            });
            binding.Bind(joinInButton, () => viewModel.OnInputChanged("a"));
            /*binding.RevertBind(slider, viewModel.Process);
            
            binding.Bind (img, viewModel.Path);
            binding.BindData(viewModel.Visible, viewModel.OnToggleChanged);
            binding.RevertBind(joinToggle, viewModel.Visible);
            binding.RevertBind(atkInputField, viewModel.ATK, (string str) => int.Parse(str));*/
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }
    }
}