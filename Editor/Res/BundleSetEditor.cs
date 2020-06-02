using System;
using SoUtil.Editor.Show;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Editor
{
    [CustomEditor(typeof(BundleSet))]
    public class BundleSetEditor : UnityEditor.Editor
    {
        private SerializedProperty m_dirs;
        private ReorderableList dirs;
        
        private SerializedProperty m_prefabs;
        private ReorderableList prefabs;
        
        private SerializedProperty m_scenes;
        private ReorderableList scenes;

        private SerializedProperty m_excludes;
        private ReorderableList excludes;
        
        float space = 0.5f;
        float lineHeight = EditorGUIUtility.singleLineHeight;
        private bool dirFolderOut = true;
        private bool prefabFolderOut = true;
        private bool sceneFolderOut = true;
        private bool excludesFolderOut = true;
        private bool genClear = false;
        private bool genInfo = true;

        private void OnEnable()
        {
            m_dirs = serializedObject.FindProperty("resDirs");
            dirs = new ReorderableList(serializedObject,m_dirs,true,true,true,true);
            dirs.drawHeaderCallback = DrawDirListHeader;
            dirs.drawElementCallback = DrawDirListElement;
            
            m_prefabs = serializedObject.FindProperty("prefabs");
            prefabs = new ReorderableList(serializedObject,m_prefabs,true,true,true,true);
            prefabs.drawHeaderCallback = DrawPrefabListHeader;
            prefabs.drawElementCallback = DrawPrefabListElement;
            
            m_scenes = serializedObject.FindProperty("scenes");
            scenes = new ReorderableList(serializedObject,m_scenes,true,true,true,true);
            scenes.drawHeaderCallback = DrawSceneListHeader;
            scenes.drawElementCallback = DrawSceneListElement;

            m_excludes = serializedObject.FindProperty("excludeRes");
            excludes = new ReorderableList(serializedObject,m_excludes,true,true,true,true);
            excludes.drawHeaderCallback = DrawExcludeListHeader;
            excludes.drawElementCallback = DrawExcludeListElement;
        }

        private float[] GetDirListColumnWidth(Rect rect)
        {
            float width5 = 40;
            float width4 = Mathf.Min(80,rect.width/5);
            float width3 = width4;
            float width2 = Mathf.Min(200, (rect.width - width3 - width4 - width5) / 2);
            float width1 = rect.width - width2 - width3 - width4 - width5;
            return new[] {width1, width2, width3, width4, width5};
        }
        
        private void DrawDirListHeader(Rect rect)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListHeaderIndex(rect);

            float[] widths = GetDirListColumnWidth(rect);
            
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), "路径");
            rect.x += widths[0];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), "文件后缀");
            rect.x += widths[1];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[2]  - space, lineHeight), "打包方案");
            rect.x += widths[2];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[3] - space, lineHeight), "引用要求");
            rect.x += widths[3];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[4] - space, lineHeight), "忽略");
        }
        
        private void DrawDirListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListIndex(rect, m_dirs, index);

            SerializedProperty m_dir = m_dirs.GetArrayElementAtIndex(index);
            SerializedProperty m_path = m_dir.FindPropertyRelative("path");
            SerializedProperty m_FilePattern = m_dir.FindPropertyRelative("filePattern");
            SerializedProperty m_bundleType = m_dir.FindPropertyRelative("bundleType");
            SerializedProperty m_bundleRef = m_dir.FindPropertyRelative("bundleRef");
            SerializedProperty m_ignore = m_dir.FindPropertyRelative("ignore");

            float[] widths = GetDirListColumnWidth(rect);
            
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), m_path, GUIContent.none);
            rect.x += widths[0];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), m_FilePattern, GUIContent.none);
            rect.x += widths[1];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[2] - space, lineHeight), m_bundleType, GUIContent.none);
            rect.x += widths[2];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[3] - space, lineHeight), m_bundleRef, GUIContent.none);
            rect.x += widths[3];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[4] - space, lineHeight), m_ignore, GUIContent.none);
        }
        
        private float[] GetOtherListColumnWidth(Rect rect)
        {
            float width2 = 40;
            float width1 = rect.width - width2;
            return new[] {width1, width2};
        }
        private void DrawPrefabListHeader(Rect rect)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListHeaderIndex(rect);
            float[] widths = GetOtherListColumnWidth(rect);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), "路径");
            rect.x += widths[0];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), "忽略");
        }
        
        private void DrawPrefabListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListIndex(rect, m_prefabs, index);

            SerializedProperty m_prefab = m_prefabs.GetArrayElementAtIndex(index);
            SerializedProperty m_path = m_prefab.FindPropertyRelative("path");
            SerializedProperty m_ignore = m_prefab.FindPropertyRelative("ignore");

            float[] widths = GetOtherListColumnWidth(rect);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), m_path, GUIContent.none);
            rect.x += widths[0];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), m_ignore, GUIContent.none);
        }
        
        private void DrawSceneListHeader(Rect rect)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListHeaderIndex(rect);
            float[] widths = GetOtherListColumnWidth(rect); 
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), "路径");
            rect.x += widths[0];
            EditorGUI.LabelField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), "忽略");
        }
        
        private void DrawSceneListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListIndex(rect, m_scenes, index);

            SerializedProperty m_scene = m_scenes.GetArrayElementAtIndex(index);
            SerializedProperty m_path = m_scene.FindPropertyRelative("path");
            SerializedProperty m_ignore = m_scene.FindPropertyRelative("ignore");

            float[] widths = GetOtherListColumnWidth(rect);
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[0] - space, lineHeight), m_path, GUIContent.none);
            rect.x += widths[0];
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, widths[1] - space, lineHeight), m_ignore, GUIContent.none);
        }
        
        private void DrawExcludeListHeader(Rect rect)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListHeaderIndex(rect);
            float width = Mathf.Min(200, rect.width);
            float residue = rect.width - width;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, width + residue - space, lineHeight), "资源路径(非目录)");
        }
        
        private void DrawExcludeListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.y += 1;
            rect = EditorGUIUtil.DrawReorderableListIndex(rect, m_excludes, index);

            SerializedProperty m_exclude = m_excludes.GetArrayElementAtIndex(index);

            float width = Mathf.Min(200, rect.width);
            float residue = rect.width - width;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width + residue - space, lineHeight), m_exclude, GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ShowUtil.ScriptTitle(target);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outputDir"), new GUIContent("输出目录"));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("seperateShader"), new GUIContent("shader分离"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shaderUnique"), new GUIContent("shader包合一"));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            dirFolderOut = EditorGUILayout.Foldout(dirFolderOut, string.Format("基础资源 ({0})", dirs.count));
            if (dirFolderOut) dirs.DoLayoutList();
            
            prefabFolderOut = EditorGUILayout.Foldout(prefabFolderOut, string.Format("预制体 ({0})", prefabs.count));
            if (prefabFolderOut) prefabs.DoLayoutList();
            
            sceneFolderOut = EditorGUILayout.Foldout(sceneFolderOut, string.Format("场景 ({0})", scenes.count));
            if (sceneFolderOut) scenes.DoLayoutList();

            excludesFolderOut = EditorGUILayout.Foldout(excludesFolderOut, string.Format("排除 ({0})", excludes.count));
            if(excludesFolderOut) excludes.DoLayoutList();
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ignoreResPattern"));
            
            EditorGUILayout.LabelField("注：以上资源获取，排除以_开头的目录和文件");
            
            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("svcExportFolder"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("svcSplitFolder"));
            
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("usePathId"));

            if(EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            
            BundleSet bundleSet = target as BundleSet;

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Gen Bundle"))
            {
                EditorApplication.delayCall += () =>
                {
                    if(EditorUtility.DisplayDialog("Gen Bundle", "是否确认要生成所有的AssetBundle?", "Yes", "No"))
                        bundleSet.Gen(genClear,genInfo);
                };
            }

            genClear = GUILayout.Toggle(genClear,"重置bundle");
            genInfo = GUILayout.Toggle(genInfo, "输出信息");
            GUILayout.EndHorizontal();
        }
    }
}