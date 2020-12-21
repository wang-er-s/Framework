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
        private Text nameMessageText => Find<Text>("name");
        private Text mulBindText => Find<Text>("mulBind");
        private InputField atkInputField => Find<InputField>("InputField");
        private Toggle joinToggle => Find<Toggle>("Toggle");
        private Button joinInButton => Find<Button>("Button");
        private Image img => Find<Image>("Image");
        private Slider slider => Find<Slider>("Slider");
        private Dropdown dropDown => Find<Dropdown>("Dropdown");
        private SetupViewModel vm;
        private View subView;

        protected override void OnVmChange()
        {
            vm = ViewModel as SetupViewModel;
            Binding.Bind(nameMessageText, vm.Visible);
            Binding.Bind(nameMessageText, vm.Process, process => $"进度为:{process}");
            Binding.Bind(mulBindText, vm.Name, vm.ATK,
                (name, atk) => $"name = {name} atk = {atk.ToString()}", (str) => mulBindText.text = $"111{str}");
            //如果绑定的是Action 则需要用闭包，下面修改OnClick方法才会实时应用上，因为Action传递是新建了一个Action并把方法传过去
            //可以把Action当成是值类型
            Binding.BindCommand(joinInButton,()=> vm.OnClick());
            Binding.RevertBind(slider, vm.Process);
            Binding.Bind(img, vm.Path);
            Binding.BindData(vm.Visible, vm.OnToggleChanged);
            Binding.RevertBind(joinToggle, vm.Visible);
            Binding.RevertBind(atkInputField, vm.ATK, (string str) => int.Parse(str));
            Binding.BindDropDown(dropDown, vm.SelectedIndex, vm.Datas);
            vm.OnClick += () => Debug.Log(222);
            Debug.Log(vm.OnClick.GetHashCode());
        }

        public override UILevel UILevel { get; } = UILevel.Common;
        public override string Path { get; } = "SimpleBind";
    }
}