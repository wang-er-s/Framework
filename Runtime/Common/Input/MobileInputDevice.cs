using UnityEngine;

namespace Framework
{
    public class MobileInputDevice : InputDeviceBase
    {
        public override bool CanUse()
        {
            return FApplication.IsMobile;
        }

        public override bool GetButton(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButton(0);
                    break;
                case InputAxisType.MouseRight:
                    return false;
                case InputAxisType.MouseMiddle:
                    return false;
                    break;
                case InputAxisType.MouseLeftDoubleClick:
                    MouseLeftDoubleClick();
                    break;
            }
            return false;
        }

        public override bool GetButtonDown(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButtonDown(0);
                    break;
                case InputAxisType.MouseRight:
                    return false;
                    break;
                case InputAxisType.MouseMiddle:
                    return false;
                    break;
            }
            return false;
        }

        public override bool GetButtonUp(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButtonUp(0);
                    break;
                case InputAxisType.MouseRight:
                    return false;
                    break;
                case InputAxisType.MouseMiddle:
                    return false;
                    break;
            }
            return false;
        }
        
        private Vector2 startPos;
        protected override float Horizontal()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    return startPos.x - touch.position.x;
            }
            return 0;
        }

        protected override float Vertical()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    return startPos.y - touch.position.y;
            }
            return 0;
        }

        protected override float MouseX()
        {
            return Horizontal();
        }

        protected override float MouseY()
        {
            return Vertical();
        }

        private float startDistance;

        protected override float MouseScrollWheel()
        {
            if (Input.touchCount != 2) return 0;
            var touch1 = Input.GetTouch(0);
            var touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                startDistance = Vector2.Distance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                var newDis = Vector2.Distance(touch1.position, touch2.position);
                var result = newDis - startDistance;
                startDistance = newDis;
                return result;
            }
            return 0;
        }
    }
}