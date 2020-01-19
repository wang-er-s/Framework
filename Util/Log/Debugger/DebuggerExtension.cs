using System.Diagnostics;
using System.Reflection;

namespace Framework
{
    public static class DebuggerExtension
    {

        public static void Log(this object obj, object message)
        {
            if (!Debugger.EnableLog) return;
            Debugger.Log(GetLogTag(obj), GetLogCallerMethod(), message.ToString(), obj);
        }

        public static void LogError(this object obj, object message)
        {
            Debugger.Error(GetLogTag(obj), GetLogCallerMethod(), message.ToString(), obj);
        }
        
        public static void LogWarning(this object obj, object message)
        {
            if (!Debugger.EnableLog) return;
            Debugger.Warning(GetLogTag(obj), GetLogCallerMethod(), message.ToString(), obj);
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
                    ms_Assembly = typeof(Debugger).Assembly;
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
