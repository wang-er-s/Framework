using System.Collections;
using System.Collections.Generic;
using Framework.Execution;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework
{
    public class InputManager
    {
        private static IInputDevice currentDevice;

        static InputManager()
        {
            if(!Application.isPlaying) return;
            var devices = new List<IInputDevice>()
            {
                new MobileInputDevice(),
                new StandardInputDevice(),
            };
            foreach (var inputDevice in devices)
            {
                if (inputDevice.CanUse())
                {
                    currentDevice = inputDevice;
                    break;
                }
            }
            Executors.RunOnCoroutineNoReturn(Update());
        }
        
        private const float doubleClickTime = 0.3f;
        private static float mouseLeftClickTime;

        public static bool MouseLeftDoubleClick()
        {
            if (Input.GetMouseButtonDown(0))
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

        public static bool TouchedUI => EventSystem.current.IsPointerOverGameObject();

        public static float GetAxis(string name)
        {
            return currentDevice.GetAxis(name);
        }

        /// <summary>
        /// 限制到-1 1之间
        /// </summary>
        public static float GetAxisClamp(string name)
        {
            var result = currentDevice.GetAxis(name);
            result = Mathf.Clamp(result, -1, 1);
            return result;
        }

        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public static float GetAxisRaw(string name)
        {
            var result = currentDevice.GetAxis(name);
            result = result == 0 ? 0 : Mathf.Sign(result);
            return result;
        }

        static IEnumerator Update()
        {
            while (true)
            {
                currentDevice.Update();
                yield return null;
            }
        }
    }
}