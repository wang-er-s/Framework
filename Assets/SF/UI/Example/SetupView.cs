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
        public Image img;
        public SetupViewModel ViewModel => Data;

        private void Awake()
        {
            //Bind(nameMessageText, (vm) => vm.Visible).OneWay();
            //Bind(nameMessageText, (vm) => vm.Name).OneWay();
            //Bind(mulBindText, (vm) => vm.Name, (vm) => vm.ATK, (name, atk) => $"name = {name} atk = {atk.ToString()}").OneWay();
            BindCommand(joinInButton, (vm) => vm.OnButtonClick).Wrap(callback =>
            {
                return () =>
                {
                    callback();
                    print("Wrap 按钮");
                };
            }).OneWay();
            Bind(img, (vm) => vm.Path).OneWay();
            //Bind(joinToggle, (vm) => vm.Visible).Wrap((value) =>
            //  {
            //      Log.I($"改为{value}");
            //      return value;
            //  }).Revert();
            //Bind(atkInputField, (vm) => vm.Name).Wrap((value) =>
            //{
            //    Log.I($"改为{value}");
            //    return value;
            //}).TwoWay();
            BindCommand<InputField, string>(atkInputField, (vm) => vm.OnInputChanged).Wrap((valueChangedFunc) =>
            {
                return (value) =>
                {
                    valueChangedFunc(value);
                    print("Wrap InputField");
                };
            }).OneWay();
        }

        protected override void OnInitialize()
        {

        }
    }
}