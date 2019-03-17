/*
* Create by Soso
* Time : 2018-12-08-09 下午
*/
using UnityEngine;
using System;

namespace ZFramework
{
	public class Platform  
	{

        public static bool IsIpad()
        {
            bool isLandscape = Screen.width > Screen.height;
            float aspect;
            if (isLandscape)
                aspect = (float)Screen.width / Screen.height;
            else
                aspect = (float)Screen.height / Screen.width;
            if (aspect > (4.0f / 3 - 0.05) && aspect < (4.0f))
                return true;
            return false;
        }

	}
}
