using Nine.Core.Message;
using Nine.UI.Core;
using UnityEngine;

namespace Nine.UI.Example
{
    public class SetupViewModel : ViewModel
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

        private string path;
        public string Path
        {
            get => path;
            set => Set(ref path, value);
        }

        public BindableList<ItemViewModel> Items { get; private set; }

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
            Items.Add(new ItemViewModel() {Path = "img"});
        }
    }

    public class ItemViewModel : ViewModel
    {
        private string path;

        public string Path
        {
            get => path;
            set => Set(ref path, value);
        }
    }

}