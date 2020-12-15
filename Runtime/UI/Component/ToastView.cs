using Framework.UI.Core;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    public class ToastView : View
    {
        public Text Text;
        public override UILevel UILevel { get; }
        public override string Path { get; }
        public override bool IsSingle { get; } = false;

        protected override void OnVmChange()
        {
             
        }

    }
}