using Framework.UI.Core;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.UI;

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
        public BindableList<Dropdown.OptionData> Datas;
        public BindableProperty<int> SelectedIndex;

        public SetupViewModel()
        {
            Name = new BindableProperty<string>();
            Job = new BindableProperty<string>();
            ATK = new BindableProperty<int>();
            SuccessRage = new BindableProperty<float>();
            Visible = new BindableProperty<bool>(false);
            Path = new BindableProperty<string>("梅菜扣肉");
            Process = new BindableProperty<float>(0.5f);
            SelectedIndex = new BindableProperty<int>(1);
            Datas = new BindableList<Dropdown.OptionData>()
            {
                new Dropdown.OptionData("First"),
                new Dropdown.OptionData("Second"),
                new Dropdown.OptionData("Third"),
            };
            SelectedIndex.AddListener(OnDropDownChanged);
        }

        private void OnDropDownChanged(int index)
        {
            Name.Value = Datas[index].text;
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
            Debug.Log($"按钮点击了{GetHashCode()}");
        }

        public override string ViewPath { get; } = "SimpleBind";

        public static SetupViewModel Create(VMCreator vmCreator)
        {
            var vm = new SetupViewModel();
            vmCreator?.BindView(vm);
            return vm;
        }
    }
}