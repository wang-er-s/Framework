#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Framework
{
    public class ConvertLine : EditorWindow
    {
        enum LineEnd
        {
            Unix_Linux,
            Windows,
            Mac,
        }

        private string extension = "cs";
        private string fileOrFolderPath;
        private LineEnd lineEnd = LineEnd.Windows;
        private string lineEndStr;
        private List<string> logs = new List<string>();

        [MenuItem("Tools/ConvertLine")]
        public static void Open()
        {
            GetWindow<ConvertLine>();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("选择文件夹"))
            {
                fileOrFolderPath = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "gkd");
            }
            if (GUILayout.Button("选择文件"))
            {
                fileOrFolderPath = EditorUtility.OpenFilePanel("选择文件", Application.dataPath, extension);
            }
            EditorGUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(fileOrFolderPath))
                EditorGUILayout.LabelField("Path:", fileOrFolderPath);
            extension = EditorGUILayout.TextField("Extension:", extension);
            lineEnd = (LineEnd) EditorGUILayout.EnumPopup("Target:", lineEnd);
            if (GUILayout.Button("生成"))
            {
                logs.Clear();
                if (!Regex.IsMatch(extension, @"\w+"))
                {
                    extension = "cs";
                    EditorUtility.DisplayDialog("", "formate is error!", "我错了");
                    return;
                }
                SetLineEnd();
                DealFolderOrFile(fileOrFolderPath);
                if (logs.Count <= 0)
                {
                    logs.Add("没有发现文件，注意检查后缀名...");
                }
            }
            foreach (var log in logs)
            {
                EditorGUILayout.LabelField("", log);
            }
        }

        private void DealFolderOrFile(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (File.Exists(path) && path.EndsWith(extension))
            {
                ChangeLineEnd(path);
                return;
            }
            if (!Directory.Exists(path)) return;
            DirectoryInfo dir = new DirectoryInfo(path);
            var fileSystemInfos = dir.GetFileSystemInfos();
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                DealFolderOrFile(fileSystemInfo.FullName);
            }
        }

        private void SetLineEnd()
        {
            switch (lineEnd)
            {
                case LineEnd.Unix_Linux:
                    lineEndStr = "\n";
                    break;
                case LineEnd.Windows:
                    lineEndStr = "\r\n";
                    break;
                case LineEnd.Mac:
                    lineEndStr = "\r";
                    break;
            }
        }

        private void ChangeLineEnd(string path)
        {
            var content = File.ReadAllText(path);
            content = Regex.Replace(content, @"\r\n|\r|\n", lineEndStr);
            File.WriteAllText(path, content);
            logs.Add($"{path} ---> completed");
        }
    }
}
#endif