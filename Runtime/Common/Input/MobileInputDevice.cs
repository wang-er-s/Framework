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

        protected override float MouseXDistance()
        {
            return HorizontalDistance();
        }

        protected override float MouseYDistance()
        {
            return VerticalDistance();
        }

        private Vector2 horizontalStartPos;
        protected override float HorizontalDistance()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    horizontalStartPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var result = touch.position.x - horizontalStartPos.x;
                    horizontalStartPos = touch.position;
                    return result;
            }
            return 0;
        }

        private Vector2 verticalStartPos;
        protected override float VerticalDistance()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    verticalStartPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var result = touch.position.y - verticalStartPos.y;
                    verticalStartPos = touch.position;
                    return result;
            }
            return 0;
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