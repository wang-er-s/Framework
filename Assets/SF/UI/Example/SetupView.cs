using SF.UI.Core;
using UnityEngine.Events;
using UnityEngine.UI;
using State = SF.Enums.State;

namespace SF.UI.Example
{
    public class SetupView : UnityGuiView<SetupViewModel>
    {

        public Text nameMessageText;
        public Text mulBindText;
        public InputField atkInputField;
        public Toggle joinToggle;
        public Button joinInButton;

        public SetupViewModel ViewModel => Data;

        private void Awake()
        {
            Bind(nameMessageText, (vm)=>vm.Name).OneWay();
            Bind(mulBindText, (vm) => vm.Name, (vm) => vm.ATK, (name, atk) => $"name = {name} atk = {atk.ToString()}").Init();
            Bind(joinInButton, Data.OnButtonClick).Wrap(callback =>
            {
                return () =>
                {
                    callback();
                    print("Wrap 按钮");
                };
            }).Init();
            Bind(joinToggle, (vm)=>vm.Visible).Wrap((value) =>
            {
                Log.I($"改为{value}");
                return value;
            }).Revert();
            Bind(atkInputField, (vm) => vm.Name).Wrap((value) =>
            {
                Log.I($"改为{value}");
                return value;
            }).TwoWay();
            //BindCommand<InputField, string>(atkInputField, Data.OnInputChanged).Wrap((valueChangedFunc) =>
            //{
            //    return (value) =>
            //    {
            //        valueChangedFunc(value);
            //        print("Wrap InputField");
            //    };
            //}).Init();
        }

        protected override void OnInitialize()
        {

        }
    }
}