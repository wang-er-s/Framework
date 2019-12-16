using AD.UI.Core;
using UnityEngine;

namespace AD.UI.Example
{
    public class SetupViewModel : ViewModel
    {
        public BindableProperty<string> Name { get; private set; }
        public BindableProperty<string> Job { get; private set; }
        public BindableProperty<int> ATK { get; private set; }
        public BindableProperty<float> SuccessRage { get; private set; }
        public BindableProperty<bool> Visible { get; private set; }
        public BindableProperty<string> Path { get; private set; }
        public BindableProperty<float> Process { get; private set; }

        public SetupViewModel()
        {
            Name = new BindableProperty<string>();
            Job = new BindableProperty<string>();
            ATK = new BindableProperty<int>();
            SuccessRage = new BindableProperty<float>();
            Visible = new BindableProperty<bool>(false);
            Path = new BindableProperty<string>("梅菜扣肉");
            Process = new BindableProperty<float>(0.5f);
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