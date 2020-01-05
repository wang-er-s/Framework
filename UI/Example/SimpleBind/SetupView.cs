using System;
using AD.UI.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AD.UI.Example
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
      
        protected override void OnVmChange ()
        {
            viewModel = VM as SetupViewModel;
            BindFactory<SetupView, SetupViewModel> binding =
                new BindFactory<SetupView, SetupViewModel> (this, viewModel);
            binding.Bind(nameMessageText, viewModel.Visible);
            binding.Bind(nameMessageText, viewModel.Process, process => $"进度为:{process}");
            //binding.Bind(nameMessageText, viewModel.Name,  process => $"进度为:{process}");
            binding.Bind (mulBindText, viewModel.Name, viewModel.ATK,
                          (name, atk) => $"name = {name} atk = {atk.ToString ()}",(str)=>mulBindText.text = $"111{str}");
            binding.Bind(joinInButton, viewModel.OnButtonClick, wrapFunc: click => () =>
            {
                click();
                print("Wrap Button");
            });
            binding.Bind(joinInButton, () => viewModel.OnInputChanged("a"));
            binding.RevertBind(slider, viewModel.Process);
            
            binding.Bind (img, viewModel.Path);
            binding.BindData(viewModel.Visible, viewModel.OnToggleChanged);
            binding.RevertBind(joinToggle, viewModel.Visible);
            binding.RevertBind(atkInputField, viewModel.ATK, (string str) => int.Parse(str));
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }
    }
}