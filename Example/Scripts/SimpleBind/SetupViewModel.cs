using System;
using Framework.UI.Core;
using Framework.UI.Core.Bind;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Example
{
    public class SetupViewModel : ViewModel
    {
        public ObservableProperty<string> Name;
        public ObservableProperty<string> Job;
        public ObservableProperty<int> ATK;
        public ObservableProperty<float> SuccessRage;
        public ObservableProperty<bool> Visible;
        public ObservableProperty<string> Path;
        public ObservableProperty<float> Process;
        public ObservableList<Dropdown.OptionData> Datas;
        public ObservableProperty<int> SelectedIndex;

        public Action OnClick;

        public SetupViewModel()
        {
            OnClick += () => Debug.Log(111);
            Name = new ObservableProperty<string>();
            Job = new ObservableProperty<string>();
            ATK = new ObservableProperty<int>();
            SuccessRage = new ObservableProperty<float>();
            Visible = new ObservableProperty<bool>(false);
            Path = new ObservableProperty<string>("回锅肉");
            Process = new ObservableProperty<float>(0.5f);
            SelectedIndex = new ObservableProperty<int>(1);
            Datas = new ObservableList<Dropdown.OptionData>()
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
    }
}