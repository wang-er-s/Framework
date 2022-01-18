using System;
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

        public static bool GetButton(string name)
        {
            return currentDevice.GetButton(name);
        }
        
        public static bool TouchedUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public static bool GetButtonDown(string name)
        {
            return currentDevice.GetButtonDown(name);
        }

        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public static bool GetButtonUp(string name)
        {
            return currentDevice.GetButtonUp(name);
        }

        public static float GetAxis(string name)
        {
            return currentDevice.GetAxis(name, false);
        }

        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public static float GetAxisRaw(string name)
        {
            return currentDevice.GetAxis(name, true);
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