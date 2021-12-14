using UnityEngine;

namespace Framework
{
    public abstract class InputDeviceBase : IInputDevice
    {
        public abstract bool CanUse();
        public virtual bool GetButton(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButton(0);
                    break;
                case InputAxisType.MouseRight:
                    return Input.GetMouseButton(1);
                    break;
                case InputAxisType.MouseMiddle:
                    return Input.GetMouseButton(2);
                    break;
                case InputAxisType.MouseLeftDoubleClick:
                    MouseLeftDoubleClick();
                    break;
            }
            return false;
        }

        private float mouseLeftClickTime;
        private const float doubleClickTime = 0.3f;
        protected bool MouseLeftDoubleClick()
        {
            if (GetButton(InputAxisType.MouseLeft))
            {
                if (Time.time - mouseLeftClickTime < doubleClickTime)
                {
                    mouseLeftClickTime = 0;
                    return true;
                }
                else
                {
                    mouseLeftClickTime = Time.time;
                }
            }
            return false;
        }
        
        public virtual bool GetButtonDown(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButtonDown(0);
                    break;
                case InputAxisType.MouseRight:
                    return Input.GetMouseButtonDown(1);
                    break;
                case InputAxisType.MouseMiddle:
                    return Input.GetMouseButtonDown(2);
                    break;
            }
            return false;
        }

        public virtual bool GetButtonUp(string name)
        {
            switch (name)
            {
                case InputAxisType.MouseLeft:
                    return Input.GetMouseButtonUp(0);
                    break;
                case InputAxisType.MouseRight:
                    return Input.GetMouseButtonUp(1);
                    break;
                case InputAxisType.MouseMiddle:
                    return Input.GetMouseButtonUp(2);
                    break;
            }
            return false;
        }

        public float GetAxis(string name, bool raw)
        {
            float result = 0;
            switch (name)
            {
                case InputAxisType.MouseX:
                    result = MouseX();
                    break;
                case InputAxisType.MouseY:
                    result = MouseY();
                    break;
                case InputAxisType.MouseScrollWheel:
                    result = MouseScrollWheel();
                    break;
                case InputAxisType.Horizontal:
                    result = Horizontal();
                    break;
                case InputAxisType.Vertical:
                    result = Vertical();
                    break;
            }
            result = Mathf.Clamp(result, -1, 1);
            if (raw)
                result = result == 0 ? 0 : Mathf.Sign(result);
            return result;
        }

        protected virtual float MouseX()
        {
            return Input.GetAxis("Mouse X");
        }
        
        protected virtual float MouseY()
        {
            return Input.GetAxis("Mouse Y");
        }

        protected virtual float MouseScrollWheel()
        {
            return Input.GetAxis("Mouse ScrollWheel");
        }

        protected virtual float Horizontal()
        {
            return Input.GetAxis("Horizontal");
        }

        protected virtual float Vertical()
        {
            return Input.GetAxis("Vertical");
        }
    }
    

    public static class InputAxisType
    {
        /// <summary>
        /// 鼠标左键
        /// </summary>
        public const string MouseLeft = "MouseLeft";
        /// <summary>
        /// 鼠标右键
        /// </summary>
        public const string MouseRight = "MouseRight";
        /// <summary>
        /// 鼠标中键
        /// </summary>
        public const string MouseMiddle = "MouseMiddle";
        /// <summary>
        /// 鼠标左键双击
        /// </summary>
        public const string MouseLeftDoubleClick = "MouseLeftDoubleClick";
        /// <summary>
        /// 鼠标X轴移动
        /// </summary>
        public const string MouseX = "MouseX";
        /// <summary>
        /// 鼠标Y轴移动
        /// </summary>
        public const string MouseY = "MouseY";
        /// <summary>
        /// 鼠标滚轮滚动
        /// </summary>
        public const string MouseScrollWheel = "MouseScrollWheel";
        /// <summary>
        /// 键盘水平输入
        /// </summary>
        public const string Horizontal = "Horizontal";
        /// <summary>
        /// 键盘垂直输入
        /// </summary>
        public const string Vertical = "Vertical";
    }
}