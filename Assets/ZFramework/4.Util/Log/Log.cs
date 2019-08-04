/*
* Create by Soso
* Time : 2018-12-08-09 下午
*/
using UnityEngine;
using System;

namespace SF
{
	public static class Log  
	{

        public static bool IsOpenLog = true;

        public static void W(object obj, params object[] args)
        {
            if (IsOpenLog)
                Debug.LogWarning(obj);
        }

        public static void E(object obj, params object[] args)
        {
            if (IsOpenLog)
                Debug.LogError(obj);
        }

        public static void I(object obj, params object[] args)
        {
            if (IsOpenLog)
                Debug.Log(obj);
        }
    }
}
