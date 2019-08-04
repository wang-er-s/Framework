using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SF
{

    public class DebuggerLog : MonoBehaviour
    {
        private class LogInfo
        {
            public LogType type;

            public string desc;

            public int times;

            public LogInfo ( LogType type, string desc )
            {
                this.type = type;
                this.desc = desc;
                times = 1;
            }
        }

        //错误详情
        private List<LogInfo> m_logEntries = new List<LogInfo> ();

        private List<LogInfo> m_logLog = new List<LogInfo> ();

        private List<LogInfo> m_logWarning = new List<LogInfo> ();

        private List<LogInfo> m_logError = new List<LogInfo> ();

        private List<LogInfo> curLog = new List<LogInfo> ();
        
        //是否显示错误窗口
        public static bool m_IsVisible = true;

        //窗口显示区域
        private Rect m_WindowRect = new Rect ( 0, 0, Screen.width, Screen.height );

        //窗口滚动区域
        private Vector2 m_scrollPositionText = Vector2.zero;

        //字体大小
        private int fontSize = 16;

        private void Start ()
        {
            curLog = m_logEntries;
            ////监听错误
            Application.logMessageReceivedThreaded += ( condition, stackTrace, type ) =>
            {
                switch ( type )
                {
                    case LogType.Warning:
                        AddItem ( ref m_logWarning, new LogInfo ( type, $"{condition}\n{stackTrace}" ) );
                        break;
                    case LogType.Log:
                        AddItem ( ref m_logLog, new LogInfo ( type, $"{condition}\n{stackTrace}" ) );
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        AddItem ( ref m_logError, new LogInfo ( type, $"{condition}\n{stackTrace}" ) );
                        break;
                }
                AddItem ( ref m_logEntries, new LogInfo ( type, $"{condition}\n{stackTrace}" ) );
            };
        }

        private void AddItem (ref List<LogInfo> list, LogInfo logInfo )
        {
            for ( int i = 0; i < list.Count; i++ )
            {
                if ( list[ i ].desc == logInfo.desc )
                {
                    list[ i ].times++;
                    return;
                }
            }
            list.Add(logInfo);
        }

        private void OnGUI ()
        {
            if ( m_IsVisible )
            {
                m_WindowRect = new Rect ( 0, 0, Screen.width, Screen.height );
                
                m_WindowRect = GUILayout.Window ( 0, m_WindowRect, ConsoleWindow, "Console" );
            }
        }

        //日志窗口
        private void ConsoleWindow ( int windowID )
        {
            GUILayout.BeginHorizontal ();
            GUI.skin.button.fontSize   = fontSize;
            GUI.skin.textArea.fontSize = fontSize;
            if ( GUILayout.Button ( "Clear", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                m_logEntries.Clear ();
            }

            if ( GUILayout.Button ( "Close", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                m_IsVisible = false;
            }

            if ( GUILayout.Button ( "AddFontSize", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                fontSize++;
            }

            if ( GUILayout.Button ( "ReduceFontSize", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                fontSize--;
            }
            GUILayout.EndHorizontal ();
            GUILayout.BeginHorizontal ();
            if ( GUILayout.Button ( "Log", GUI.skin.button, GUILayout.MaxWidth ( 200 ), GUILayout.MaxHeight ( 100 ) ) )
            {
                curLog = curLog == m_logLog ? m_logEntries : m_logLog;
            }

            if ( GUILayout.Button ( "Warning", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                curLog = curLog == m_logWarning ? m_logEntries : m_logWarning;
            }

            if ( GUILayout.Button ( "Error", GUI.skin.button, GUILayout.MaxWidth ( 200 ),
                                    GUILayout.MaxHeight ( 100 ) ) )
            {
                curLog = curLog == m_logError ? m_logEntries : m_logError;
            }
            GUILayout.EndHorizontal ();
            GUI.skin.verticalScrollbar.fixedWidth = 50;
            //GUI.skin.verticalScrollbar.stretchWidth = true;
            m_scrollPositionText =
                GUILayout.BeginScrollView ( m_scrollPositionText, GUI.skin.horizontalScrollbar,
                                            GUI.skin.verticalScrollbar );
            foreach ( var entry in curLog )
            {
                Color currentColor = GUI.contentColor;
                switch ( entry.type )
                {
                    case LogType.Warning:
                        GUI.contentColor = Color.white;
                        break;
                    case LogType.Assert:
                        GUI.contentColor = Color.black;
                        break;
                    case LogType.Log:
                        GUI.contentColor = Color.green;
                        break;
                    case LogType.Error:
                    case LogType.Exception:
                        GUI.contentColor = Color.red;
                        break;
                }

                GUILayout.Label ( entry.times + "  " + entry.desc, GUI.skin.textArea );
                GUI.contentColor = currentColor;
            }

            GUILayout.EndScrollView ();
        }

    }
}

