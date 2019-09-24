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
            Bind(nameMessageText, Data.Name).Init();
            //Bind<Text, string, int, string>(mulBindText, Data.Name, Data.ATK).For(data => mulBindText.text = data).Wrap((name, atk) => $"name={name},atk={atk}").Init();
            //Bind(joinInButton, Data.OnButtonClick).For(joinInButton.onClick).Wrap(callback =>
            //{
            //    return () =>
            //    {
            //        callback();
            //        print("Wrap °´Å¥");
            //    };
            //}).Init();
            //Bind<Toggle, bool>(joinToggle, Data.OnToggleChanged).For(joinToggle.onValueChanged).Wrap((valueChangedFunc) =>
            //{
            //    return (value) =>
            //    {
            //        valueChangedFunc(value);
            //        print("Wrap Toggle");
            //    };
            //}).Init();
            //Bind<InputField, string>(atkInputField, Data.OnInputChanged).For(atkInputField.onValueChanged).Wrap((valueChangedFunc) =>
            //{
            //    return (value) =>
            //    {
            //        valueChangedFunc(value);
            //        print("Wrap InputField");
            //    };
            //}).Init();
            //Bind(joinToggle, Data.Visible).For(isShow => joinToggle.isOn = isShow).Init();
        }

        protected override void OnInitialize()
        {

        }
    }
}