using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Object = UnityEngine.Object;

#if FIX_LOG
namespace Framework
{
    public static class LogExtension
    {

        public static void Msg(this Object obj, params object[] msgs)
        {
            if (!Log.EnableLog) return;
            Log.Msg(obj, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }

        public static void Error(this Object obj, params object[] msgs)
        {
            Log.Error(obj, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }
        
        public static void Warning(this Object obj, params object[] msgs)
        {
            if (!Log.EnableLog) return;
            Log.Warning(obj, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }
        
        public static void Msg(this object obj, params object[] msgs)
        {
            if (!Log.EnableLog) return;
            Log.Msg(null, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }

        public static void Error(this object obj, params object[] msgs)
        {
            Log.Error(null, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }

        public static void Warning(this object obj, params object[] msgs)
        {
            if (!Log.EnableLog) return;
            Log.Warning(null, GetLogTag(obj), GetLogCallerMethod(), msgs);
        }

        //----------------------------------------------------------------------



        //----------------------------------------------------------------------
        private static string GetLogTag(object obj)
        {
            FieldInfo fi = obj.GetType().GetField("LOG_TAG");
            if (fi != null)
            {
                return (string) fi.GetValue(obj);
            }

            return obj.GetType().Name;
        }

        private static Assembly ms_Assembly;
        private static string GetLogCallerMethod()
        {
            StackTrace st = new StackTrace(2, false);
            if (st != null)
            {
                if (null == ms_Assembly)
                {
                    ms_Assembly = typeof(Log).Assembly;
                }

                int currStackFrameIndex = 0;
                while (currStackFrameIndex < st.FrameCount)
                {
                    StackFrame oneSf = st.GetFrame(currStackFrameIndex);
                    MethodBase oneMethod = oneSf.GetMethod();

                    if (oneMethod.Module.Assembly != ms_Assembly)
                    {
                        return oneMethod.Name;
                    }

                    currStackFrameIndex++;
                }

            }

            return "";
        }
    }
}
#endif