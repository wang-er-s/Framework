using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
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
        public Dropdown dropDown;
        public SetupViewModel vm;
        public View subView;
        private UIBindFactory<SetupView, SetupViewModel> binding;

        protected override void OnVmChange()
        {
            vm = ViewModel as SetupViewModel;
            if (binding == null)
                binding = new UIBindFactory<SetupView, SetupViewModel>(this, vm);
            binding.UpdateVm(vm);
            binding.Bind(nameMessageText, vm.Visible);
            binding.Bind(nameMessageText, vm.Process, process => $"进度为:{process}");
            binding.Bind(mulBindText, vm.Name, vm.ATK,
                (name, atk) => $"name = {name} atk = {atk.ToString()}", (str) => mulBindText.text = $"111{str}");
            //如果绑定的是Action 则需要用闭包，下面修改OnClick方法才会实时应用上，因为Action传递是新建了一个Action并把方法传过去
            //可以把Action当成是值类型
            binding.BindCommand(joinInButton,()=> vm.OnClick());
            binding.RevertBind(slider, vm.Process);
            binding.Bind(img, vm.Path);
            binding.BindData(vm.Visible, vm.OnToggleChanged);
            binding.RevertBind(joinToggle, vm.Visible);
            binding.RevertBind(atkInputField, vm.ATK, (string str) => int.Parse(str));
            binding.BindDropDown(dropDown, vm.SelectedIndex, vm.Datas);
            vm.OnClick += () => print(222);
            Debug.Log(vm.OnClick.GetHashCode());
        }

        public override UILevel UILevel { get; } = UILevel.Common;
    }
}