using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ScriptTemplateCreate : EditorWindow
{
    class Func
    {
        public string funcName;

        public bool isOverride;

        public string returnType;

        public List<KeyValuePair<string, string>> paraList;

        public Func ( string className, string returnType, bool isOverride, List<KeyValuePair<string, string>> paras )
        {
            this.funcName   = className;
            this.returnType = returnType;
            this.isOverride = isOverride;
            this.paraList   = paras;
        }
    }

    private string scriptName = "";

    private bool awake = false;

    private bool start = false;

    private bool update = false;

    private bool isMono = true;

    private string path = "Assets";

    private string extendClass = "";

    private Vector2 m_scrollPositionText;

    [MenuItem ( "Assets/创建脚本模版" )]
    static void CreateScriptTemplate ()
    {
        EditorWindow.GetWindow ( typeof ( ScriptTemplateCreate ) );
    }

    private void OnGUI ()
    {
        GUILayout.BeginVertical ();
        m_scrollPositionText =
            GUILayout.BeginScrollView ( m_scrollPositionText, GUI.skin.horizontalScrollbar,
                                        GUI.skin.verticalScrollbar );
        GUI.skin.label.fontSize  = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label ( "Create Script" );
        GUILayout.Space ( 10 );
        GUI.skin.label.fontSize = 12;
        //GUILayout.Label("Script Name");
        scriptName = EditorGUILayout.TextField ( "Script Name", scriptName );
        awake      = EditorGUILayout.Toggle ( "Awake", awake );
        start      = EditorGUILayout.Toggle ( "Start", start );
        update     = EditorGUILayout.Toggle ( "Update", update );
        isMono     = EditorGUILayout.Toggle ( "IsMono", isMono );
        //GUILayout.FlexibleSpace();

        if ( GUILayout.Button ( "Create" ) )
        {
            var obj = Selection.activeObject;
            path = obj ? AssetDatabase.GetAssetPath ( obj.GetInstanceID () ) : "Assets";
            if ( !Directory.Exists ( path ) )
            {
                string[] strs = path.Split ( '/' );
                path = "";
                for ( int i = 0; i < strs.Length - 1; i++ )
                {
                    path += strs[ i ] + "/";
                }
            }

            Create ();
            EditorWindow.GetWindow ( typeof ( ScriptTemplateCreate ) ).Close ();
        }

        GUILayout.EndScrollView ();
        GUILayout.EndVertical ();
    }

    private void OnTools_OptimizeSelected ()
    {
        throw new NotImplementedException ();
    }

    void Create ()
    {
        StreamWriter  sw = new StreamWriter ( path + "/" + scriptName + ".cs" );
        StringBuilder sb = new StringBuilder ();
        sb.AppendLine ( "/*" );
        sb.AppendLine ( "* Create by Soso" );
        sb.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
        sb.AppendLine ( "*/" );
        sb.AppendLine ( "using UnityEngine;" );
        sb.AppendLine ( "using System;" );
        sb.AppendLine ( "using ZFramework;" );
        sb.AppendLine ();
        sb.AppendFormat ( "public class {0}  ", scriptName );
        if ( isMono )
            sb.Append ( ": MonoBehaviour" );
        else if ( !string.IsNullOrEmpty ( extendClass ) )
            sb.Append ( $": {extendClass}" );
        sb.AppendLine ();
        sb.AppendLine ( "{" );
        if ( awake )
        {
            sb.AppendLine ( "\tprivate void Awake()" );
            sb.AppendLine ( "\t{" );
            sb.AppendLine ();
            sb.AppendLine ( "\t}" );
            sb.AppendLine ();
        }

        if ( start )
        {
            sb.AppendLine ( "\tprivate void Start()" );
            sb.AppendLine ( "\t{" );
            sb.AppendLine ();
            sb.AppendLine ( "\t}" );
            sb.AppendLine ();
        }

        if ( update )
        {
            sb.AppendLine ( "\tprivate void Update()" );
            sb.AppendLine ( "\t{" );
            sb.AppendLine ();
            sb.AppendLine ( "\t}" );
        }

        sb.AppendLine ();
        sb.AppendLine ( "}" );
        sw.Write ( sb );
        sw.Flush ();
        sw.Close ();
        AssetDatabase.SaveAssets ();
        AssetDatabase.Refresh ();
    }

    string MatchType ( Type type )
    {
        if ( type == typeof ( void ) )
            return "void";
        if ( type == typeof ( string ) )
            return "string";
        if ( type == typeof ( bool ) )
            return "bool";
        if ( type == typeof ( int ) )
            return "int";
        if ( type == typeof ( long ) )
            return "long";
        if ( type == typeof ( float ) )
            return "float";
        if ( type == typeof ( byte ) )
            return "byte";
        if ( type == typeof ( ushort ) )
            return "ushort";
        if ( type == typeof ( object ) )
            return "object";
        return type.Name;
    }

}