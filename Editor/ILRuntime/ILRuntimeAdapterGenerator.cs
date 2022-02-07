using System;
using System.Diagnostics;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if ILRUNTIME

namespace Framework.Editor
{
    [Serializable]
    public class ILRuntimeAdapterGenerator
    {
        [ShowInInspector]
        private string assemblyName = "Assembly-CSharp";
        [ShowInInspector]
        private string adapterClassName = "";
        private string OUTPUT_PATH;

        public void Init()
        {
            OUTPUT_PATH = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig.AdaptorPath;
        }
        

        [Button("生成适配器",ButtonSizes.Large)]
        private void GenAdapter()
        {
            var assembly = AssemblyManager.GetAssembly(assemblyName);
            if (assembly == null)
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("程序集名找不到"));
                return;
            }
            Debug.Log(typeof(IDisposable).FullName);
            var type = assembly.GetType(adapterClassName);
            if (type == null)
            {
                EditorWindow.focusedWindow.ShowNotification(new GUIContent("类名找不到，检查一下命名空间和名字"));
                return;
            }
            GenOneAdapter(type);
            AssetDatabase.Refresh();
        }

        private void GenOneAdapter(Type type)
        {
            if (!Directory.Exists(OUTPUT_PATH))
            {
                Directory.CreateDirectory(OUTPUT_PATH);
            }

            //如果有先删除
            if (File.Exists($"{OUTPUT_PATH}/{type}Adapter.cs"))
            {
                File.Delete($"{OUTPUT_PATH}/{type}Adapter.cs");
                if (File.Exists($"{OUTPUT_PATH}/{type}Adapter.cs.meta"))
                {
                    File.Delete($"{OUTPUT_PATH}/{type}Adapter.cs.meta");
                }
                AssetDatabase.Refresh();
            }

            //生成适配器
            FileStream stream =
                new FileStream($"{OUTPUT_PATH}/{type}Adapter.cs", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(stream);
            Stopwatch watch = new Stopwatch();
            sw.WriteLine(
                ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(type, "Adapter"));
            watch.Stop();
            Log.Msg($"Generated {OUTPUT_PATH}/{type}Adapter.cs in: " +
                    watch.ElapsedMilliseconds + " ms.");
            sw.Dispose();
        }
    }
}
#endif