using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class InputManager : MonoSingleton<InputManager>
    {
        private IInputDevice currentDevice;

        private List<IInputDevice> devices;

        private void Awake()
        {
            devices = new List<IInputDevice>()
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
        }

        public bool GetButton(string name)
        {
            return currentDevice.GetButton(name);
        }

        /// <summary>
        /// 按钮按下
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否按下</returns>
        public bool GetButtonDown(string name)
        {
            return currentDevice.GetButtonDown(name);
        }

        /// <summary>
        /// 按钮抬起
        /// </summary>
        /// <param name="name">按钮名称</param>
        /// <returns>是否抬起</returns>
        public bool GetButtonUp(string name)
        {
            return currentDevice.GetButtonUp(name);
        }

        public float GetAxis(string name)
        {
            return currentDevice.GetAxis(name, false);
        }

        /// <summary>
        /// 获取轴线值（值为-1，0，1）
        /// </summary>
        /// <param name="name">轴线名称</param>
        /// <returns>值</returns>
        public float GetAxisRaw(string name)
        {
            return currentDevice.GetAxis(name, true);
        }

        private void Update()
        {
            currentDevice.Update();
        }
    }
}