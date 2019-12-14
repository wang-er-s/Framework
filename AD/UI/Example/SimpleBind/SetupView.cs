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
            //nameMessa|geText show or hide by vm.visible
            binding.Bind (nameMessageText, viewModel.Visible);
            //nameMessageText.text show text by vm.Name
            binding.Bind (nameMessageText, viewModel.Name);
            //mulBindText.text show text by (vm.ATK , vm.Name) , and wrap by third para
            binding.Bind (mulBindText, viewModel.Name, viewModel.ATK,
                          (name, atk) => $"name = {name} atk = {atk.ToString ()}");
            //button bind vm.OnButtonClick
            binding.BindCommand (joinInButton, viewModel.OnButtonClick).Wrap (callback =>
            {
                return () =>
                {
                    callback ();
                    print ("Wrap 按钮");
                };
            });
            binding.TwoWayBind(slider, viewModel.Process);
            //image bind path, when path changed, img.sprite change to res.load(path)
            binding.Bind (img, viewModel.Path);
            binding.Bind (joinToggle, viewModel.Visible).Wrap ((value) =>
            {
                Debug.Log ($"改为{value}");
                return value;
            }).Revert ();
            binding.TwoWayBind (atkInputField, viewModel.ATK).Wrap ((value) =>
            {
                Debug.Log ($"改为{value}");
                return value;
            });
            binding.BindCommand<InputField, string> (atkInputField, viewModel.OnInputChanged).Wrap (valueChangedFunc =>
            {
                return (value) =>
                {
                    valueChangedFunc (value);
                    print ("Wrap InputField");
                };
            });
            //binding.BindList (viewModel.Items, item1, item2).Init();
            AddSubView(subView);
            binding.InitBind();
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }
    }
}