using UnityEngine;

namespace Framework
{
    public class StandardInputDevice : InputDeviceBase
    {
        public override bool CanUse()
        {
            return FApplication.IsEditor || FApplication.IsPC;
        }

        private Vector2 mousePosDelta;
        protected override float MouseX()
        {
            return mousePosDelta.x;
        }
        
        protected override float MouseY()
        {
            return mousePosDelta.y;
        }

        protected override float MouseScrollWheel()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }

        protected override float Horizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        public StandardInputDevice()
        {
            mousePosDelta = Vector2.zero;
            lastMousePos = Input.mousePosition;
        }

        private Vector3 lastMousePos;
        public override void Update()
        {
            mousePosDelta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;
        }
        
        protected override float Vertical()
        {
            return Input.GetAxis("Vertical");
        }
    }
}