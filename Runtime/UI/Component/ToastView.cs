using Framework.UI.Core;
using TMPro;
using UnityEngine.UI;

namespace Framework.Runtime.UI.Component
{
    [UI("ToastContent")]
    public class ToastContent : View
    {
        protected override void OnVmChange()
        {
        }

        public override UILevel UILevel { get; } = UILevel.Toast;
    }
    
    [UI("Toast")]
    public class ToastView : View
    {
        [TransformPath("content/Text")]
        public Text Text;
        public override UILevel UILevel { get; } = UILevel.Toast;
        public override bool IsSingle { get; } = false;

        public LayoutElement LayoutElement { get; private set; }

        protected override void Start()
        {
            LayoutElement = Go.GetComponent<LayoutElement>();
        }

        protected override void OnVmChange()
        {
             
        }
    }
}