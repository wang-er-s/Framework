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
        
        public BindableList<ItemViewModel> Items { get; private set; }
        

        public void OnToggleChanged ( bool value )
        {
            Debug.Log ( value );
        }

        public void OnInputChanged ( string name )
        {
            Debug.Log ( name );
        }

        public void OnButtonClick ()
        {
            Debug.Log ( "按钮点击了" );
        }

        public override void OnCreate ()
        {
        }
    }

    public class ItemViewModel : ViewModel , IBindMulView
    {
        
        public BindableProperty<string> Path { get; private set; }

        public int Tag { get; set; }

        public override void OnCreate ()
        {
            
        }
    }
}