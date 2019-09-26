using SF.Core.Message;
using SF.UI.Core;
using UnityEngine;

namespace SF.UI.Example
{
    public class SetupViewModel : ViewModelBase
    {
        private string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        private string job;
        public string Job
        {
            get => job;
            set => Set(ref job, value);
        }

        private int atk;
        public int ATK
        {
            get => atk;
            set => Set(ref atk, value);
        }

        private float successRate;
        public float SuccessRate
        {
            get => successRate;
            set => Set(ref successRate, value);
        }

        private bool visible;
        public bool Visible
        {
            get => visible;
            set => Set(ref visible, value);
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

        public override void OnCreate()
        {

        }
    }
}