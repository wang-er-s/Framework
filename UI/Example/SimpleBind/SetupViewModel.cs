using Framework.UI.Core;
using UnityEngine;

namespace Framework.UI.Example
{
    public class SetupViewModel : ViewModel
    {
        public BindableProperty<string> Name;
        public BindableProperty<string> Job;
        public BindableProperty<int> ATK;
        public BindableProperty<float> SuccessRage;
        public BindableProperty<bool> Visible;
        public BindableProperty<string> Path;
        public BindableProperty<float> Process;

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
            Debug.Log($"按钮点击了{this.GetHashCode()}");
        }

        public override string ViewPath { get; } = "SimpleBind";

        public static SetupViewModel Create(VMCreator vmCreator)
        {
            SetupViewModel vm = new SetupViewModel();
            vmCreator?.BindView(vm);
            return vm;
        }

    }
}