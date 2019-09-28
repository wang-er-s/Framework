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
            //Bind(nameMessageText, Data.Name).Init();
            //Bind<Text, string, int, string>(mulBindText, Data.Name, Data.ATK).Wrap((name, atk) => $"name={name},atk={atk}").Init();
            //Bind(joinInButton, Data.OnButtonClick).Wrap(callback =>
            //{
            //    return () =>
            //    {
            //        callback();
            //        print("Wrap °´Å¥");
            //    };
            //}).Init();
            //Bind(joinToggle, joinToggle.isOn).Init();
            ////Bind<InputField, string>(atkInputField, Data.OnInputChanged).Wrap((valueChangedFunc) =>
            ////{
            ////    return (value) =>
            ////    {
            ////        valueChangedFunc(value);
            ////        print("Wrap InputField");
            ////    };
            ////}).Init();
            //Bind(joinToggle, Data.Visible).Init();
        }

        protected override void OnInitialize()
        {

        }
    }
}