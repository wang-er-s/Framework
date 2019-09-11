using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CSTest2
{
    class ActionBinding
    {
        public void Main()
        {
            Button btn = new Button();
            btn.Text = new Text();
            BtnViewModel viewModel = new BtnViewModel();
            Binding<BtnViewModel> binding = new Binding<BtnViewModel>();
            viewModel.Name = new BindData<string>("啊啊啊", null);
            binding.BindComponent<Text, string>(btn.Text, ((data) => btn.Text.TextValue = data)).Set(viewModel.Name);
            Stopwatch s1 = Stopwatch.StartNew();
            s1.Start();
            //Type type = viewModel.GetType();
            for (int i = 0; i < 10000; i++)
            {
                //viewModel.Name.Set("哈哈哈"+i);
                viewModel.Name2 = ("哈哈哈" + i);
                btn.Text.TextValue = viewModel.Name2;
            }
            s1.Stop();
            Console.WriteLine(s1.ElapsedMilliseconds);
        }
    }
    class Binding<TViewModel>
    {
        public BindComponent<TComponent, TData> BindComponent<TComponent, TData>(TComponent component, Action<TData> data)
        {
            return new BindComponent<TComponent, TData>(component, data);
        }
    }

    class BindComponent<TComponent, TData>
    {
        private string PropertyName;
        private TComponent component;
        private Action<TData> ValueChangeEvent;

        public BindComponent(TComponent component, Action<TData> data)
        {
            this.component = component;
            ValueChangeEvent = data;
        }

        public void Set(BindData<TData> data)
        {
            data.AddChangeEvent((value) => ValueChangeEvent(value));
        }
    }

    class Button
    {
        public Text Text;
        public Action OnClick;
    }

    class Text
    {
        private string text;

        public string TextValue
        {
            get { return text; }
            set
            {
                text = value;
                //Console.WriteLine("设置了text的值 " + text);
            }
        }
    }

    class BtnViewModel
    {

        public BindData<string> Name;
        public string Name2;
        public bool Visible;
        public float Height;

        public bool B => true;

        public BtnViewModel()
        {
            Name = new BindData<string>("ZZZ", null);
        }

        public void OnClick()
        {
            Console.WriteLine("点击了");
        }

    }

    class BindData<TData>
    {
        public event Action<TData> DataChangeEvent;

        private TData data;

        public BindData(TData data, Action<TData> changeAction)
        {
            this.data = data;
            DataChangeEvent = changeAction;
        }

        public void AddChangeEvent(Action<TData> changeAction)
        {
            if (DataChangeEvent == null)
                DataChangeEvent = changeAction;
            else
                DataChangeEvent += changeAction;
        }

        public void Set(TData _data)
        {
            if (data.Equals(_data)) return;
            data = _data;
            DataChangeEvent?.Invoke(_data);
        }

        public TData Get => data;

    }
}
