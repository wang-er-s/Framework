using Framework.UI.Core;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    public class ToastView : View
    {
        public Text Text;
        public override UILevel UILevel { get; }
        
        protected override void OnVmChange()
        {
             
        }

        public override string ViewPath { get; }
    }
}