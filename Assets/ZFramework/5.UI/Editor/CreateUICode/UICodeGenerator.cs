using System;
using System.Text;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
using Object = UnityEngine.Object;

namespace ZFramework
{

    public class PanelCodeData
    {
        public string PanelName;
        public Dictionary<string, string> DicNameToFullName = new Dictionary<string, string> ();
        public readonly List<MarkedObjInfo> MarkedObjInfos = new List<MarkedObjInfo> ();
        public readonly List<ElementCodeData> ElementCodeDatas = new List<ElementCodeData> ();
    }

    public class ElementCodeData
    {
        public MarkedObjInfo MarkedObjInfo;
        public string BehaviourName;
        public Dictionary<string, string> DicNameToFullName = new Dictionary<string, string> ();
        public readonly List<MarkedObjInfo> MarkedObjInfos = new List<MarkedObjInfo> ();
        public readonly List<ElementCodeData> ElementCodeDatas = new List<ElementCodeData> ();
    }

    /// <summary>
    /// 存储一些Mark相关的信息
    /// </summary>
    public class MarkedObjInfo
    {
        public string Name;

        public string PathToElement;

        public UIMark MarkObj;
    }

    public class UICodeGenerator
    {
        [MenuItem ( "Assets/@UI Kit - Create UIPanelCode" )]
        public static void CreateUICode ()
        {
            var objs =
                Selection.GetFiltered ( typeof ( GameObject ), SelectionMode.Assets | SelectionMode.TopLevel );
            var displayProgress = objs.Length > 1;
            if ( displayProgress ) EditorUtility.DisplayProgressBar ( "", "Create UIPrefab Code...", 0 );
            for ( var i = 0; i < objs.Length; i++ )
            {
                mInstance.CreateCode ( objs[ i ] as GameObject, AssetDatabase.GetAssetPath ( objs[ i ] ) );
                if ( displayProgress )
                    EditorUtility.DisplayProgressBar ( "", "Create UIPrefab Code...", (float) ( i + 1 ) / objs.Length );
            }

            AssetDatabase.Refresh ();
            if ( displayProgress ) EditorUtility.ClearProgressBar ();
        }
        
        private void CreateCode ( GameObject obj, string uiPrefabPath )
        {
            if ( obj != null )
            {
                var prefabType = PrefabUtility.GetPrefabType ( obj );
                if ( PrefabType.Prefab != prefabType )
                {
                    return;
                }

                var clone = PrefabUtility.InstantiatePrefab ( obj ) as GameObject;
                if ( null == clone )
                {
                    return;
                }
                FindAllMarkTrans ( clone.transform, "" );
                mPanelCodeData = new PanelCodeData ();
                Debug.Log ( clone.name );
                mPanelCodeData.PanelName = clone.name.Replace ( "(clone)", string.Empty );
                CreateUIPanelCode ( obj, uiPrefabPath );
                AddSerializeUIPrefab ( obj, mPanelCodeData );
                Object.DestroyImmediate ( clone );
            }
        }

        private static string PathToParent ( Transform trans, string parentName )
        {
            var retValue = new StringBuilder ( trans.name );

            while ( trans.parent != null )
            {
                if ( trans.parent.name.Equals ( parentName ) )
                {
                    break;
                }

                retValue = trans.parent.name.Append ( "/" ).Append ( retValue );

                trans = trans.parent;
            }

            return retValue.ToString ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootTrans"></param>
        /// <param name="curTrans"></param>
        /// <param name="transFullName"></param>
        private void FindAllMarkTrans ( Transform curTrans, string transFullName,
                                        ElementCodeData parentElementCodeData = null )
        {
            foreach ( Transform childTrans in curTrans )
            {
                var uiMark = childTrans.GetComponent<UIMark> ();

                if ( null != uiMark )
                {
                    if ( null == parentElementCodeData )
                    {
                        if ( !mPanelCodeData.MarkedObjInfos.Any ( markedObjInfo =>
                                                                      markedObjInfo.Name.Equals (
                                                                          uiMark.Transform.name ) )
                        )
                        {
                            mPanelCodeData.MarkedObjInfos.Add ( new MarkedObjInfo
                            {
                                Name          = uiMark.Transform.name,
                                MarkObj       = uiMark,
                                PathToElement = PathToParent ( childTrans, mPanelCodeData.PanelName )
                            } );
                            mPanelCodeData.DicNameToFullName.Add ( uiMark.Transform.name,
                                                                   transFullName + childTrans.name );
                        }
                        else
                        {
                            Debug.LogError ( "Repeat key: " + childTrans.name );
                        }
                    }
                    else
                    {
                        if ( !parentElementCodeData.MarkedObjInfos.Any (
                                 markedObjInfo =>
                                     markedObjInfo.Name.Equals ( uiMark.Transform.name ) ) )
                        {
                            parentElementCodeData.MarkedObjInfos.Add ( new MarkedObjInfo ()
                            {
                                Name          = uiMark.Transform.name,
                                MarkObj       = uiMark,
                                PathToElement = PathToParent ( childTrans, parentElementCodeData.BehaviourName )
                            } );
                            parentElementCodeData.DicNameToFullName.Add ( uiMark.Transform.name,
                                                                          transFullName + childTrans.name );
                        }
                        else
                        {
                            Debug.LogError ( "Repeat key: " + childTrans.name );
                        }
                    }
                    FindAllMarkTrans ( childTrans, transFullName + childTrans.name + "/", parentElementCodeData );
                }
                else
                {
                    FindAllMarkTrans ( childTrans, transFullName + childTrans.name + "/", parentElementCodeData );
                }
            }
        }

        private void CreateUIPanelCode ( GameObject uiPrefab, string uiPrefabPath )
        {
            if ( null == uiPrefab )
                return;

            var behaviourName = uiPrefab.name;

            var strFilePath = string.Empty;

            strFilePath = GetScriptsPath () + "/" + GetLastName ( uiPrefabPath );

            strFilePath.Replace ( uiPrefab.name + ".prefab", string.Empty ).CreateDirIfNotExists ();

            strFilePath = strFilePath.Replace ( ".prefab", ".cs" );

            if ( File.Exists ( strFilePath ) == false )
            {
                UIPanelCodeTemplate.Generate ( strFilePath, behaviourName, GetProjectNamespace () );
            }

            CreateUIPanelComponentsCode ( behaviourName, strFilePath );
            Debug.Log ( ">>>>>>>Success Create UIPrefab Code: " + behaviourName );
        }

        public static string GetLastName ( string absOrAssetsPath )
        {
            var name = absOrAssetsPath.Replace ( "\\", "/" );
            var dirs = name.Split ( '/' );

            return dirs[ dirs.Length - 1 ];
        }

        private void CreateUIPanelComponentsCode ( string behaviourName, string uiUIPanelfilePath )
        {
            var dir              = uiUIPanelfilePath.Replace ( behaviourName + ".cs", "" );
            var generateFilePath = dir + behaviourName + "Components.cs";

            UIPanelComponentsCodeTemplate.Generate ( generateFilePath, behaviourName, GetProjectNamespace (),
                                                     mPanelCodeData );

            foreach ( var elementCodeData in mPanelCodeData.ElementCodeDatas )
            {
                var elementDir = string.Empty;
                elementDir = elementCodeData.MarkedObjInfo.MarkObj.GetUIMarkType () == UIMarkType.Element
                                 ? ( dir + behaviourName + "/" ).CreateDirIfNotExists ()
                                 : ( Application.dataPath + "/" + GetScriptsPath () + "/Components/" ).
                                 CreateDirIfNotExists ();
                CreateUIElementCode ( elementDir, elementCodeData );
            }
        }

        private static void CreateUIElementCode ( string generateDirPath, ElementCodeData elementCodeData )
        {
            if ( File.Exists ( generateDirPath + elementCodeData.BehaviourName + ".cs" ) == false )
            {
                UIElementCodeTemplate.Generate ( generateDirPath + elementCodeData.BehaviourName + ".cs",
                                                 elementCodeData.BehaviourName, GetProjectNamespace (),
                                                 elementCodeData );
            }

            UIElementCodeComponentTemplate.Generate ( generateDirPath + elementCodeData.BehaviourName + "Components.cs",
                                                      elementCodeData.BehaviourName, GetProjectNamespace (),
                                                      elementCodeData );

            foreach ( var childElementCodeData in elementCodeData.ElementCodeDatas )
            {
                var elementDir = ( generateDirPath + elementCodeData.BehaviourName + "/" ).CreateDirIfNotExists ();
                CreateUIElementCode ( elementDir, childElementCodeData );
            }
        }

        private static void AddSerializeUIPrefab ( GameObject uiPrefab, PanelCodeData panelData )
        {
            var prefabPath = AssetDatabase.GetAssetPath ( uiPrefab );
            if ( string.IsNullOrEmpty ( prefabPath ) )
                return;

            var pathStr = EditorPrefs.GetString ( "AutoGenUIPrefabPath" );
            if ( string.IsNullOrEmpty ( pathStr ) )
            {
                pathStr = prefabPath;
            }
            else
            {
                pathStr += ";" + prefabPath;
            }

            EditorPrefs.SetString ( "AutoGenUIPrefabPath", pathStr );
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void SerializeUIPrefab ()
        {
            var pathStr = EditorPrefs.GetString ( "AutoGenUIPrefabPath" );
            if ( string.IsNullOrEmpty ( pathStr ) )
                return;

            EditorPrefs.DeleteKey ( "AutoGenUIPrefabPath" );
            Debug.Log ( ">>>>>>>SerializeUIPrefab: " + pathStr );

            var assembly = ReflectionExtension.GetAssemblyCSharp ();

            var paths           = pathStr.Split ( new[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
            var displayProgress = paths.Length > 3;
            if ( displayProgress ) EditorUtility.DisplayProgressBar ( "", "Serialize UIPrefab...", 0 );

            for ( var i = 0; i < paths.Length; i++ )
            {
                var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject> ( paths[ i ] );
                AttachSerializeObj ( uiPrefab, uiPrefab.name, assembly );

                // uibehaviour
                if ( displayProgress )
                    EditorUtility.DisplayProgressBar ( "", "Serialize UIPrefab..." + uiPrefab.name,
                                                       (float) ( i + 1 ) / paths.Length );
                Debug.Log ( ">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name );
            }

            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();

            for ( var i = 0; i < paths.Length; i++ )
            {
                var uiPrefab = AssetDatabase.LoadAssetAtPath<GameObject> ( paths[ i ] );
                AttachSerializeObj ( uiPrefab, uiPrefab.name, assembly );

                // uibehaviour
                if ( displayProgress )
                    EditorUtility.DisplayProgressBar ( "", "Serialize UIPrefab..." + uiPrefab.name,
                                                       (float) ( i + 1 ) / paths.Length );
                Debug.Log ( ">>>>>>>Success Serialize UIPrefab: " + uiPrefab.name );
            }

            AssetDatabase.SaveAssets ();
            AssetDatabase.Refresh ();

            if ( displayProgress ) EditorUtility.ClearProgressBar ();

        }

        private static void AttachSerializeObj ( GameObject obj, string behaviourName,
                                                 System.Reflection.Assembly assembly,
                                                 List<UIMark> processedMarks = null )
        {
            if ( null == processedMarks )
            {
                processedMarks = new List<UIMark> ();
            }

            var uiMark    = obj.GetComponent<UIMark> ();
            var className = string.Empty;

            if ( uiMark != null )
            {
                className = GetProjectNamespace () + "." + uiMark.ComponentName;

                // 这部分
                if ( uiMark.GetUIMarkType () != UIMarkType.DefaultUnityElement )
                {
                    var ptuimark = obj.GetComponent<UIMark> ();
                    if ( ptuimark != null )
                    {
                        UnityEngine.Object.DestroyImmediate ( ptuimark, true );
                    }
                }
            }
            else
            {
                className = GetProjectNamespace () + "." + behaviourName;
            }

            //			Debug.Log(">>>>>>>Class Name: " + className);
            var t = assembly.GetType ( className );

            var com     = obj.GetComponent ( t ) ?? obj.AddComponent ( t );
            var sObj    = new SerializedObject ( com );
            var uiMarks = obj.GetComponentsInChildren<UIMark> ( true );

            foreach ( var elementMark in uiMarks )
            {
                if ( processedMarks.Contains ( elementMark )
                     || elementMark.GetUIMarkType () == UIMarkType.DefaultUnityElement )
                {
                    continue;
                }

                processedMarks.Add ( elementMark );

                var uiType       = elementMark.ComponentName;
                var propertyName = string.Format ( "{0}", elementMark.Transform.gameObject.name );

                if ( sObj.FindProperty ( propertyName ) == null )
                {
                    Debug.Log ( string.Format ( "sObj is Null:{0} {1} {2}", propertyName, uiType, sObj ) );
                    continue;
                }

                sObj.FindProperty ( propertyName ).objectReferenceValue = elementMark.Transform.gameObject;
                AttachSerializeObj ( elementMark.Transform.gameObject, elementMark.ComponentName, assembly,
                                     processedMarks );
            }

            var marks = obj.GetComponentsInChildren<UIMark> ( true );
            foreach ( var elementMark in marks )
            {
                if ( processedMarks.Contains ( elementMark ) )
                {
                    continue;
                }

                processedMarks.Add ( elementMark );

                var propertyName = elementMark.Transform.name;
                sObj.FindProperty ( propertyName ).objectReferenceValue = elementMark.Transform.gameObject;
            }
            sObj.FindProperty("canvasGroup").objectReferenceValue = obj.GetOrAddComponent<CanvasGroup>();
            sObj.ApplyModifiedPropertiesWithoutUndo ();
        }

        private static string GetScriptsPath ()
        {
            //return FrameworkSettingData.Load().UIScriptDir;
            return "Assets/Scripts/UIPanel";
        }

        private static string GetProjectNamespace ()
        {
            //return FrameworkSettingData.Load().Namespace;
            //TODO 创建UI代码添加自定义命名空间
            return "";
        }

        private PanelCodeData mPanelCodeData;
        
        private ElementCodeData mElementCodeData;

        private static readonly UICodeGenerator mInstance = new UICodeGenerator ();
    }
}