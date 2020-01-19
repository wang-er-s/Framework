using System;
using System.Diagnostics;
using System.IO;

namespace Framework
{
    public class Debugger
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

        static Debugger()
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

        private static void Internal_Log(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.Log(msg, (UnityEngine.Object) context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogWarning(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogWarning(msg, (UnityEngine.Object) context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }

        private static void Internal_LogError(string msg, object context = null)
        {
            if (UseUnityEngine)
            {
                UnityEngine.Debug.LogError(msg, (UnityEngine.Object) context);
            }
            else
            {
                Console.WriteLine(msg);
            }
        }


        //----------------------------------------------------------------------
        internal static void Log(string tag, string methodName, string message, object content = null)
        {
            if (!EnableLog) return;
            message = GetLogText(tag, methodName, message);
            Log(message, content);
        }

        internal static void Error(string tag, string methodName, string message, object content = null)
        {
            message = GetLogText(tag, methodName, message);
            Error(message, content);
        }

        internal static void Warning(string tag, string methodName, string message, object content = null)
        {
            if (!EnableLog) return;
            message = GetLogText(tag, methodName, message);
            Warning(message, content);
        }
        
        public static void Log(object message, Object context = null )
        {
            if (!EnableLog) return;
            Internal_Log(Prefix + message, context);
            LogToFile("[I]" + message);
        }

        public static void Error(object message, Object context = null)
        {
            Internal_LogError(Prefix + message, context);
            LogToFile("[E]" + message, true);
        }
        
        public static void Warning(object message, Object context = null)
        {
            if (!EnableLog) return;
            Internal_LogWarning(Prefix + message, context);
            LogToFile("[W]" + message);
        }

        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }
        
        //----------------------------------------------------------------------
        private static string GetLogText(string tag, string methodName, string message)
        {
            string str = "";
            if (EnableTime)
            {
                DateTime now = DateTime.Now;
                str = now.ToString("HH:mm:ss.fff") + " ";
            }

            str = str + tag + "::" + methodName + "() " + message;
            return str;
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
