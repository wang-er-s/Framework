using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class ProjectScanTools
    {
        public static Texture2D CreateRwTexture2D(Texture2D texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary( 
                texture.width,
                texture.height,
                0,
                RenderTextureFormat.Default,
                RenderTextureReadWrite.Linear);
            Graphics.Blit(texture, tmp);
            // Backup the currently set RenderTexture
            RenderTexture previous = RenderTexture.active;
// Set the current RenderTexture to the temporary one we created
            RenderTexture.active = tmp;
// Create a new readable Texture2D to copy the pixels to it
            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
// Copy the pixels from the RenderTexture to the new Texture
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
// Reset the active RenderTexture
            RenderTexture.active = previous;
// Release the temporary RenderTexture
            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }

        public static bool IsLongAudio(AudioClip clip)
        {
            return clip.length > 10;
        }
        
        public static bool IsMediumAudio(AudioClip clip)
        {
            return clip.length > 2 && clip.length < 10;
        }
        
        public static bool IsShortAudio(AudioClip clip)
        {
            return clip.length < 2;
        }

        private static GUIStyle defaultStyle;

        public static GUIStyle DefaultStyle
        {
            get
            {
                if (defaultStyle == null)
                {
                    defaultStyle = new GUIStyle();
                    defaultStyle.wordWrap = true;
                    defaultStyle.fontSize = 14;
                    defaultStyle.normal.textColor = Color.white;
                }

                return defaultStyle;
            }
        }

        public static void DrawPathList(List<string> dirList, string name)
        {
            SirenixEditorGUI.BeginBox();
            EditorGUILayout.Space(5);
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(name, DefaultStyle);
                if (GUILayout.Button("添加+", GUILayout.Height(20), GUILayout.Width(50)))
                {
                    OpenFolderPanel(dirList);
                }
            }

            EditorGUILayout.Space(5);

            for (int i = 0; i < dirList.Count; i++)
            {
                SirenixEditorGUI.BeginBox(GUILayout.Height(25));
                using (new GUILayout.HorizontalScope())
                {
                    string include = dirList[i];
                    EditorGUILayout.LabelField(include, defaultStyle);
                    if (GUILayout.Button("删除-", GUILayout.Height(20), GUILayout.Width(50)))
                    {
                        dirList.RemoveAt(i);
                        i--;
                    }
                }

                SirenixEditorGUI.EndBox();
            }

            EditorGUILayout.Space(5);
            SirenixEditorGUI.EndBox();
        }

        private static async void OpenFolderPanel(List<string> addList)
        {
            // 需要等待一下 不然odin会报错
            await Task.Delay(10);
            var selected = EditorUtility.OpenFolderPanel("添加目录", Application.dataPath, "");
            addList.Add(Path.GetRelativePath(Application.dataPath, selected));
        }

        public static string ParseCSVItem(object obj)
        {
            string str = obj.ToString();
            str = str.Replace("\"", "\"\"");
            str = $"\"{str}\",";
            return str;
        }
    }

    public struct KeyValue
    {
        public object Key;
        public object Value;

        public KeyValue(object key, object value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Key}:{Value}";
        }
    }
}