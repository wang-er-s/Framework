using SF.UI.Core;
using UnityEngine.Events;
using UnityEngine.UI;
using State = SF.Enums.State;

namespace SF.UI.Example
{
    public class SetupView : UnityGuiView<SetupViewModel>
    {

        public InputField nameInputField;
        public Text nameMessageText;

        public InputField jobInputField;
        public Text jobMessageText;

        public InputField atkInputField;
        public Text atkMessageText;

        public Slider successRateSlider;
        public Text successRateMessageText;

        public Toggle joinToggle;
        public Button joinInButton;
        public Button waitButton;

        public SetupViewModel ViewModel
        {
            get { return (SetupViewModel) Data; }
        }

        protected override void OnInitialize()
        {
            Bind(nameMessageText, Data.Job).For((data) => nameMessageText.text = data);
            Bind(joinInButton, Data.OnButtonClick);
            Bind(joinToggle, Data.OnToggleChanged);
        }


        private void OnSuccessRatePropertyValueChanged(float oldValue, float newValue)
        {
            successRateMessageText.text = newValue.ToString("F2");
        }

        private void OnATKPropertyValueChanged(int oldValue, int newValue)
        {
            atkMessageText.text = newValue.ToString();
        }

        private void OnJobPropertyValueChanged(string oldValue, string newValue)
        {
            jobMessageText.text = newValue.ToString();
        }

        private void OnNamePropertyValueChanged(string oldValue, string newValue)
        {
            nameMessageText.text = newValue;
        }

        private void OnStatePropertyValueChanged(State oldValue, State newValue)
        {
            switch (newValue)
            {
                case State.JoinIn:
                    joinInButton.interactable = true;
                    waitButton.interactable = false;
                    break;
                case State.Wait:
                    joinInButton.interactable = false;
                    waitButton.interactable = true;
                    break;
            }
        }

        public void iptName_ValueChanged()
        {
            ViewModel.Name.Value = nameInputField.text;
        }

        public void iptJob_ValueChanged()
        {
            ViewModel.Job.Value = jobInputField.text;
        }

        public void iptATK_ValueChanged()
        {
            int result;
            if (int.TryParse(atkInputField.text, out result))
            {
                ViewModel.ATK.Value = int.Parse(atkInputField.text);
            }
        }

        public void sliderSuccessRate_ValueChanged()
        {
            ViewModel.SuccessRate.Value = successRateSlider.value;
        }

        public void toggle_ValueChanged()
        {
            if (joinToggle.isOn)
            {
                ViewModel.State.Value = State.JoinIn;
            }
            else
            {
                ViewModel.State.Value = State.Wait;
            }
        }

        public void JoinInBattleTeam()
        {
            ViewModel.JoininCurrentTeam();
        }

        public void JoinInClan()
        {
            ViewModel.JoininClan();
        }
    }
}