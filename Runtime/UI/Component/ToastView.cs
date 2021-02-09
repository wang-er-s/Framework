using Framework.UI.Core;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    [UI("UI_Toast")]
    public class ToastView : View
    {
        public Text Text;
        public override UILevel UILevel { get; } = UILevel.Toast;
        public override bool IsSingle { get; } = false;

        protected override void OnVmChange()
        {
             
        }

    }
}