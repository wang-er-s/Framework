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

        private Vector2 horizontalStartPos;
        protected override float Horizontal()
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