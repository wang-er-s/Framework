/*
* Create by Soso
* Time : 2018-12-08-09 下午
*/
using UnityEngine;
using System;

namespace Nine
{
    public static class Log
    {

        public static bool IsOpenLog = true;

        public static void Warning ( object obj, params object[] args )
        {
            if ( !IsOpenLog ) return;
            Debug.LogWarning ( obj );
        }

        public static void Error ( object obj, params object[] args )
        {
            if ( !IsOpenLog ) return;
            Debug.LogError ( obj );
        }

        public static void Info ( object obj, params object[] args )
        {
            if ( !IsOpenLog ) return;
            Debug.Log ( obj );
        }
    }
}
