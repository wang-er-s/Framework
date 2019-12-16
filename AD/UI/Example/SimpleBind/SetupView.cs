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
            //nameMessa|geText show or hide by vm.visible
            binding.Bind (nameMessageText, viewModel.Visible).InitBind();
            //nameMessageText.text show text by vm.Name
            binding.Bind(nameMessageText, viewModel.Process).Wrap(process => $"进度为:{process}").InitBind();
            //mulBindText.text show text by (vm.ATK , vm.Name) , and wrap by third para
            binding.Bind (mulBindText, viewModel.Name, viewModel.ATK,
                          (name, atk) => $"name = {name} atk = {atk.ToString ()}").InitBind();
            //button bind vm.OnButtonClick
            binding.BindCommand (joinInButton, viewModel.OnButtonClick).Wrap((onBtnClick => { 
                return () =>
                {
                    onBtnClick();
                    Debug.Log("点击了button");
                }; })).InitBind();
            binding.BindCommand(joinInButton, ()=> viewModel.OnInputChanged("a") ).InitBind();
            binding.TwoWayBind(slider, viewModel.Process).InitBind();
            //image bind path, when path changed, img.sprite change to res.load(path)
            binding.Bind (img, viewModel.Path);
            // Toggle control viewModel.Visible
            binding.RevertBind(joinToggle, viewModel.Visible).Wrap ((value) =>
            {
                Debug.Log ($"Toggle 改为{value}");
                return value;
            }).InitBind();
            //将inputField的值付给ATK
            binding.RevertBind(atkInputField, viewModel.ATK).Wrap<string>(val => int.Parse(val) * 2).InitBind();
        }

        protected override void OnDestroy ()
        {
            base.OnDestroy ();
        }
    }
}