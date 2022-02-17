using UnityEngine;

namespace Framework
{
    public class MobileInputDevice : InputDeviceBase
    {
        public override bool CanUse()
        {
            return FApplication.IsMobile;
        }
        
        protected override float MouseX()
        {
            return Horizontal();
        }

        protected override float MouseY()
        {
            return Vertical();
        }

        private Vector2 horizontalLastPos;
        protected override float Horizontal()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    horizontalLastPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var result = touch.position.x - horizontalLastPos.x;
                    horizontalLastPos = touch.position;
                    return result;
            }
            return 0;
        }

        private Vector2 horizontalFirstPressPos;
        protected override float HorizontalConst()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    horizontalFirstPressPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var result = touch.position.x - horizontalFirstPressPos.x;
                    return result;
            }
            return 0;
        }

        private Vector2 verticalStartPos;
        protected override float Vertical()
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

        private Vector2 verticalFirstPressPos;
        protected override float VerticalConst()
        {
            if (Input.touchCount != 1) return 0;
            var touch = Input.GetTouch(0);
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    verticalFirstPressPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var result = touch.position.y - verticalFirstPressPos.y;
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