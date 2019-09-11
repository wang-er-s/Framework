using SF.Core.Message;
using SF.UI.Core;
using UnityEngine;

namespace SF.UI.Example
{
    public class SetupViewModel : ViewModelBase
    {
        public readonly BindableProperty<string> Name = new BindableProperty<string>();
        public readonly BindableProperty<string> Job=new BindableProperty<string>(); 
        public readonly BindableProperty<int> ATK = new BindableProperty<int>();
        public readonly BindableProperty<float> SuccessRate=new BindableProperty<float>(); 
        public readonly BindableProperty<Enums.State> State=new BindableProperty<Enums.State>();


        public void JoininCurrentTeam()
        {
            MessageAggregator<object>.Instance.Publish("Toggle", this,new MessageArgs<object>("Red"));
                                               
            Debug.Log(Name.Value + "加入当前Team，职业："+Job.Value+",攻击力："+ATK.Value+"成功率："+SuccessRate.Value);
        }

        public void JoininClan()
        {
            MessageAggregator<object>.Instance.Publish("Toggle", this, new MessageArgs<object>("Yellow"));
            Debug.Log(Name.Value + "加入当前Clan，职业：" + Job.Value + ",攻击力：" + ATK.Value + "成功率：" + SuccessRate.Value);
        }

        public void OnToggleChanged(bool value)
        {
            Debug.Log(value);
        }

        public void OnButtonClick()
        {

        }

        public override void OnCreate()
        {
            
        }
    }
}