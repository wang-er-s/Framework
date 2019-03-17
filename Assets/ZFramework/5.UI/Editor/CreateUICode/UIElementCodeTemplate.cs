using System;
using UnityEngine;
using System.Text;
using System.IO;

namespace ZFramework
{


    public static class UIElementCodeTemplate
    {
        
        public static void mGenerate (string  filePath, UIElementData elementData )
        {
            var sw         = new StreamWriter ( filePath, false, new UTF8Encoding ( false ) );
            var strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );

            strBuilder.AppendLine ( "using System;" );
            strBuilder.AppendLine ( "using System.Collections.Generic;" );
            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );

            strBuilder.AppendFormat ( "public partial class {0} : MonoBehaviour",elementData.BehaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "private void Awake()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            foreach ( string btnNameList in elementData.btnNameLists )
            {
                strBuilder.Append ( "\t\t" ).AppendFormat ( $"{btnNameList}.onClick.AddListener(On{btnNameList}Click);" ).
                           AppendLine ();
            }
            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ();
            foreach ( string btnNameList in elementData.btnNameLists )
            {
                strBuilder.Append ( "\t" ).AppendFormat ( "private void On{0}Click()", btnNameList ).AppendLine ();
                strBuilder.Append ( "\t" ).AppendLine ( "{" );
                strBuilder.Append ( "\t" ).AppendLine ( "}" );
                strBuilder.AppendLine ();
            }
            
            strBuilder.AppendLine ( "}" );

            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }
        
        public static void Generate ( string generateFilePath, string behaviourName, string nameSpace,
                                      ElementCodeData elementCodeData )
        {
            var sw         = new StreamWriter ( generateFilePath, false, new UTF8Encoding ( false ) );
            var strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );

            strBuilder.AppendLine ( "using System;" );
            strBuilder.AppendLine ( "using System.Collections.Generic;" );
            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );
            
            strBuilder.AppendFormat ( "public partial class {0}", behaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "private void Awake()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void OnBeforeDestroy()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ( "}" );

            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }
    }

    public static class UIElementCodeComponentTemplate
    {

        public static void mGenerate (string filePath, UIElementData elementCodeData )
        {
            var sw         = new StreamWriter ( filePath, false, Encoding.UTF8 );
            var strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );
            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );
            strBuilder.AppendLine ( "using ZFramework;" );
            strBuilder.AppendLine ();

            strBuilder.AppendFormat ( "public partial class {0}", elementCodeData.BehaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "{" );

            foreach ( var markInfo in elementCodeData.markNameLists )
            {
                var strUIType = markInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat ( "\t[SerializeField] private {0} {1};\r\n",strUIType, markInfo.Name );
            }
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "}" );
            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }

        public static void Generate ( string generateFilePath, string behaviourName, string nameSpace,
                                      ElementCodeData elementCodeData )
        {
            var sw         = new StreamWriter ( generateFilePath, false, Encoding.UTF8 );
            var strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );
            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );
            strBuilder.AppendLine ( "using ZFramework;" );
            strBuilder.AppendLine ();

            strBuilder.AppendFormat ( "public partial class {0}", behaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "{" );

            foreach ( var markInfo in elementCodeData.MarkedObjInfos )
            {
                var strUIType = markInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat ( "\t[SerializeField] public {0} {1};\r\n",
                                          strUIType, markInfo.Name );
            }

            strBuilder.AppendLine ();

            strBuilder.Append ( "\t" ).AppendLine ( "public void Clear()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            foreach ( var markInfo in elementCodeData.MarkedObjInfos )
            {
                strBuilder.AppendFormat ( "\t{0} = null;\r\n",
                                          markInfo.Name );
            }

            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ();

            strBuilder.Append ( "\t" ).AppendLine ( "public override string ComponentName" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t\t" );
            strBuilder.AppendLine ( "get { return \"" + elementCodeData.MarkedObjInfo.MarkObj.ComponentName + "\";}" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ( "}" );
            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }
    }
}