using AD.UI.Core;
using UnityEngine;

namespace AD.UI.Example
{
    public class SetupViewModel : ViewModel
    {
        public BindableField<string> Name;
        public BindableField<string> Job;
        public BindableField<int> ATK;
        public BindableField<float> SuccessRage;
        public BindableField<bool> Visible;
        public BindableField<string> Path;
        public BindableField<float> Process;

        public SetupViewModel()
        {
            Name = new BindableField<string>();
            Job = new BindableField<string>();
            ATK = new BindableField<int>();
            SuccessRage = new BindableField<float>();
            Visible = new BindableField<bool>(false);
            Path = new BindableField<string>("梅菜扣肉");
            Process = new BindableField<float>(0.5f);
        }

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
        
    }
}