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
        protected override float MouseXDistance()
        {
            return mousePosDelta.x;
        }
        
        protected override float MouseYDistance()
        {
            return mousePosDelta.y;
        }

        private Vector2 horizontalStartPos;
        protected override float HorizontalDistance()
        {
            if (Input.GetMouseButtonDown(0))
            {
                horizontalStartPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                var result = Input.mousePosition.x - horizontalStartPos.x;
                horizontalStartPos = Input.mousePosition;
                return result;
            }
            return 0;
        }

        public StandardInputDevice()
        {
            lastMousePos = Input.mousePosition;
            mousePosDelta = lastMousePos - Input.mousePosition;
        }

        private Vector3 lastMousePos;
        public override void Update()
        {
            mousePosDelta = Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;
        }
        
        private Vector2 verticalStartPos;
        protected override float VerticalDistance()
        {
            if (Input.GetMouseButtonDown(0))
            {
                verticalStartPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                var result = Input.mousePosition.y - verticalStartPos.y;
                verticalStartPos = Input.mousePosition;
                return result;
            }
            return 0;
        }
    }
}