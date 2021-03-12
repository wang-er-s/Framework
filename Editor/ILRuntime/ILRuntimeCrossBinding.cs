#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Framework.Editor
{
    [Obfuscation(Exclude = true)]
    public class ILRuntimeCrossBinding
    {
        internal class ILRuntimeCrossBindingAdapterGenerator : OdinEditorWindow
        {
            private static ILRuntimeCrossBindingAdapterGenerator window;
            private string[] AllAssembly;
            private Type[] AllClass;
            private const string OUTPUT_PATH = "Assets/_Scripts/Adapters";

            [MenuItem("Framework/ILRuntime/生成适配器", priority = 1001)]
            public static void ShowWindow()
            {
                window = GetWindow<ILRuntimeCrossBindingAdapterGenerator>();
                window.titleContent = new GUIContent("Generate Cross bind Adapter");
                window.minSize = new Vector2(300, 150);
                window.Init();
                window.Show();
            }

            private void Init()
            {
                AllAssembly = CompilationPipeline.GetAssemblies(AssembliesType.Player)
                    .Select((assembly => assembly.name)).ToArray();
                Assembly = "GamePlay";
            }

            private string _assembly;
            [SerializeField]
            [LabelText("程序集名")]
            [ValueDropdown("AllAssembly")]
            private string Assembly
            {
                get => _assembly;
                set
                {
                    _assembly = value;
                    OnChangeAssembly();
                }
            }
            [SerializeField]
            [LabelText("类名")]
            [ValueDropdown("AllClass")]
            private Type _class;

            [Button("生成适配器")]
            private void GenAdapter()
            {
                //获取主工程DLL的类
                //Type t = AssemblyManager.GetAssembly(_assembly).GetType(_class);
                Type t = _class;

                if (!Directory.Exists(OUTPUT_PATH))
                {
                    Directory.CreateDirectory(OUTPUT_PATH);
                }

                //如果有先删除
                if (File.Exists($"{OUTPUT_PATH}/{_class}Adapter.cs"))
                {
                    File.Delete($"{OUTPUT_PATH}/{_class}Adapter.cs");
                    if (File.Exists($"{OUTPUT_PATH}/{_class}Adapter.cs.meta"))
                    {
                        File.Delete($"{OUTPUT_PATH}/{_class}Adapter.cs.meta");
                    }

                    AssetDatabase.Refresh();
                }

                //生成适配器
                FileStream stream =
                    new FileStream($"{OUTPUT_PATH}/{_class}Adapter.cs", FileMode.Append, FileAccess.Write);
                StreamWriter sw = new StreamWriter(stream);
                Stopwatch watch = new Stopwatch();
                sw.WriteLine(
                    ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(t, "Adapter"));
                watch.Stop();
                Log.Msg($"Generated {OUTPUT_PATH}/{_class}Adapter.cs in: " +
                        watch.ElapsedMilliseconds + " ms.");
                sw.Dispose();

                window.Close();

                AssetDatabase.Refresh();
            }

            private void OnChangeAssembly()
            {
                AllClass = AssemblyManager.GetTypeList(_assembly);
            }
        }
    }
#endif
}
