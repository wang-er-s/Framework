using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Object = UnityEngine.Object;
#if FIX_LOG
namespace Framework
{
    public class Log
    {
        public static bool EnableLog = true;
        public static bool EnableTime = false;
        public static bool EnableSave = false;
        public static bool EnableStack = false;
        public static string LogFileDir = "";
        public static string LogFileName = "";
        public static string Prefix = "> ";
        public static StreamWriter LogFileWriter = null;
        public static bool UseUnityEngine = true;

        static Log()
        {
            if (UseUnityEngine)
            {
                LogFileDir = UnityEngine.Application.persistentDataPath + "/DebugerLog/";
            }
            else
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                LogFileDir = path + "/DebugerLog/";
            }
        }

        private static void Internal_Log(string msg, Object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.Log(msg, context );
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogWarning(string msg, Object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogWarning(msg, context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogError(string msg, Object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogError(msg, context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }


        //----------------------------------------------------------------------
        internal static void Msg( Object content ,string tag, string methodName, params object[] msgs)
        {
            if (!EnableLog) return;
            var message = GetLogText(tag, methodName, msgs);
            Internal_Log(Prefix + message, content);
            LogToFile("[M]" + message);
        }

        internal static void Error(Object content ,string tag, string methodName, params object[] msgs)
        {
            var message = GetLogText(tag, methodName, msgs);
            Internal_LogError(Prefix + message, content);
            LogToFile("[E]" + message);
        }

        internal static void Warning(Object content ,string tag, string methodName, params object[] msgs)
        {
            if (!EnableLog) return;
            var message = GetLogText(tag, methodName, msgs);
            Internal_LogWarning(Prefix + message, content);
            LogToFile("[W]" + message);
        }
        
        public static void Msg(params object[] msgs)
        {
            Msg(null, null, null, msgs);
        }

        public static void Error(params object[] msgs)
        {
            Error(null, null, null, msgs);
        }
        
        public static void Warning(params object[] msgs)
        {
            Warning(null, null, null, msgs);
        }

        public static void Assert(bool condition, string message = "", Object context = null)
        {
            if (!condition)
                LogToFile("[A]" + message);
            UnityEngine.Debug.Assert(condition, message, context);
        }
        
        //----------------------------------------------------------------------
        private static string GetLogText(string tag, string methodName, params object[] msgs)
        {
            StringBuilder sb = new StringBuilder();
            if (EnableTime)
            {
                DateTime now = DateTime.Now;
                sb.Append(now.ToString("HH:mm:ss.fff")).Append(" ");
            }
            if (!string.IsNullOrEmpty(tag) && !string.IsNullOrEmpty(methodName))
                sb.Append(tag).Append("::").Append(methodName).Append("()--->");
            foreach (var msg in msgs)
            {
                sb.Append(msg).Append('\t');
            }
            return sb.ToString();
        }


        //----------------------------------------------------------------------
        internal static string CheckLogFileDir()
        {
            if (string.IsNullOrEmpty(LogFileDir))
            {
                //该行代码无法在线程中执行！
                try
                {
                    if (UseUnityEngine)
                    {
                        LogFileDir = UnityEngine.Application.persistentDataPath + "/DebugerLog/";
                    }
                    else
                    {
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        LogFileDir = path + "/DebugerLog/";
                    }
                }
                catch (Exception e)
                {
                    Internal_LogError("Debugger::CheckLogFileDir() " + e.Message + e.StackTrace);
                    return "";
                }
            }

            try
            {
                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }
            }
            catch (Exception e)
            {
                Internal_LogError("Debugger::CheckLogFileDir() " + e.Message + e.StackTrace);
                return "";
            }

            return LogFileDir;
        }

        internal static string GenLogFileName()
        {
            DateTime now = DateTime.Now;
            string filename = now.GetDateTimeFormats('s')[0].ToString(); //2005-11-05T14:06:25
            filename = filename.Replace("-", "_");
            filename = filename.Replace(":", "_");
            filename = filename.Replace(" ", "");
            filename += ".log";

            return filename;
        }

        private static void LogToFile(string message, bool enableStack = false)
        {
            if (!EnableSave)
            {
                return;
            }

            if (LogFileWriter == null)
            {
                LogFileName = GenLogFileName();
                LogFileDir = CheckLogFileDir();
                if (string.IsNullOrEmpty(LogFileDir))
                {
                    return;
                }

                string fullpath = LogFileDir + LogFileName;
                try
                {
                    LogFileWriter = File.AppendText(fullpath);
                    LogFileWriter.AutoFlush = true;
                }
                catch (Exception e)
                {
                    LogFileWriter = null;
                    Internal_LogError("Debugger::LogToFile() " + e.Message + e.StackTrace);
                    return;
                }
            }

            if (LogFileWriter != null)
            {
                try
                {
                    LogFileWriter.WriteLine(message);
                    if ((enableStack || EnableStack) && UseUnityEngine)
                    {
                        LogFileWriter.WriteLine(UnityEngine.StackTraceUtility.ExtractStackTrace());
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
#endif