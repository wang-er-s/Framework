using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ZFramework
{

    public static class UIPanelCodeTemplate
    {
        public static void Generate ( string generateFilePath, string behaviourName, string nameSpace )
        {
            var           sw         = new StreamWriter ( generateFilePath, false, new UTF8Encoding ( false ) );
            StringBuilder strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );

            strBuilder.AppendLine ( "using System;" );
            strBuilder.AppendLine ( "using System.Collections.Generic;" );
            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );
            strBuilder.AppendLine ( "using ZFramework;" );

            strBuilder.AppendFormat ( "public class {0}Data : UIPanelData", behaviourName ).AppendLine ();
            strBuilder.AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "// TODO: Query Mgr's Data" );
            strBuilder.AppendLine ( "}" );
            strBuilder.AppendLine ();
            strBuilder.AppendFormat ( "public partial class {0} : UIPanel", behaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void InitUI(IUIData uiData = null)" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).Append ( "\t" ).
                       AppendLine ( "mData = uiData as " + behaviourName + "Data ?? new " + behaviourName + "Data();" );
            strBuilder.Append ( "\t" ).Append ( "\t" ).AppendLine ( "//please add init code here" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void RegisterUIEvent()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void OnUpdate()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void OnShow()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void OnHide()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void OnClose()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            strBuilder.Append ( "\t" ).AppendLine ( "}" ).AppendLine ();
            strBuilder.Append ( "}" ).AppendLine ();

            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }
    }

    public static class UIPanelComponentsCodeTemplate
    {
        public static void Generate ( string generateFilePath, string behaviourName, string nameSpace,
                                      PanelCodeData panelCodeData )
        {
            var sw         = new StreamWriter ( generateFilePath, false, new UTF8Encoding ( false ) );
            var strBuilder = new StringBuilder ();

            strBuilder.AppendLine ( "/*" );
            strBuilder.AppendLine ( "* Create by Soso" );
            strBuilder.AppendLine ( "* Time : " + DateTime.Now.ToString ( "yyyy-MM-dd-hh tt" ) );
            strBuilder.AppendLine ( "*/" );

            strBuilder.AppendLine ( "using UnityEngine;" );
            strBuilder.AppendLine ( "using UnityEngine.UI;" );
            strBuilder.AppendLine ( "using ZFramework;" );
            strBuilder.AppendLine ();
            strBuilder.AppendFormat ( "public partial class {0}\n", behaviourName );
            strBuilder.AppendLine ( "{" );
            strBuilder.AppendFormat ( "\tpublic const string NAME = \"{0}\";", behaviourName ).AppendLine ();
            strBuilder.AppendLine ("\tpublic override UIMoveType MoveType { get { return UIMoveType.Fixed; } }");
            strBuilder.AppendLine ();
            
            foreach ( var objInfo in panelCodeData.MarkedObjInfos )
            {
                var strUIType = objInfo.MarkObj.ComponentName;
                strBuilder.AppendFormat ( "\t[SerializeField] private {0} {1};\r\n",
                                          strUIType, objInfo.Name );
            }

            strBuilder.AppendLine ();
            strBuilder.Append ( "\t" ).AppendLine ( "protected override void ClearUIComponents()" );
            strBuilder.Append ( "\t" ).AppendLine ( "{" );
            foreach ( var markInfo in panelCodeData.MarkedObjInfos )
            {
                strBuilder.AppendFormat ( "\t\t{0} = null;\r\n",
                                          markInfo.Name );
            }

            strBuilder.Append ( "\t" ).AppendLine ( "}" );
            strBuilder.AppendLine ();
            strBuilder.AppendFormat ( "\tprivate {0}Data mPrivateData = null;\n", behaviourName );
            strBuilder.AppendLine ();
            strBuilder.AppendFormat ( "\tpublic {0}Data mData\n", behaviourName );
            strBuilder.AppendLine ( "\t{" );
            strBuilder.Append ( "\t\tget { return mPrivateData ?? (mPrivateData = new " ).Append ( behaviourName ).
                       Append ( "Data()); }" ).AppendLine ();
            strBuilder.AppendLine ( "\t\tset" );
            strBuilder.AppendLine ( "\t\t{" );
            strBuilder.AppendLine ( "\t\t\tmUIData = value;" );
            strBuilder.AppendLine ( "\t\t\tmPrivateData = value;" );
            strBuilder.AppendLine ( "\t\t}" );
            strBuilder.AppendLine ( "\t}" );
            strBuilder.AppendLine ( "}" );

            sw.Write ( strBuilder );
            sw.Flush ();
            sw.Close ();
        }
    }

}