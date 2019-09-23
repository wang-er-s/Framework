using SF.Core.Message;
using SF.UI.Core;
using UnityEngine;

namespace SF.UI.Example
{
    public class SetupViewModel : ViewModelBase
    {
        public BindableProperty<string> Name = new BindableProperty<string>();
        public BindableProperty<string> Job=new BindableProperty<string>(); 
        public BindableProperty<int> ATK = new BindableProperty<int>();
        public BindableProperty<float> SuccessRate=new BindableProperty<float>(); 
        public BindableProperty<Enums.State> State=new BindableProperty<Enums.State>();

        public void OnToggleChanged(bool value)
        {
            Debug.Log(value);
        }

        public void OnInputChanged(string name)
        {
            Debug.Log(name);
        }

        public void OnButtonClick()
        {
            Debug.Log("按钮点击了");
        }

        public override void OnCreate()
        {
            
        }
    }
}