using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Framework.Editor
{
    [Serializable]
    public class ILRuntimeAdapterGenerator
    {
        private string[] AllAssembly;
        private Type[] AllClass;
        private const string OUTPUT_PATH = "Assets/_Scripts/ILRuntime";

        public void Init()
        {
            AllAssembly = CompilationPipeline.GetAssemblies(AssembliesType.Player)
                .Select((assembly => assembly.name)).ToArray();
            Assembly = "GamePlay";
        }

        private string _assembly;
        [ShowInInspector]
        [LabelText("程序集名")]
        [ValueDropdown("AllAssembly")]
        public string Assembly
        {
            get => _assembly;
            set
            {
                _assembly = value;
                OnChangeAssembly();
            }
        }
        
        [ShowInInspector]
        [LabelText("类名")] 
        [ValueDropdown("AllClass")]
        public Type MClass;

        [Button("生成适配器")]
        private void GenAdapter()
        {
            //获取主工程DLL的类
            //Type t = AssemblyManager.GetAssembly(_assembly).GetType(_class);
            Type t = MClass;
            if (!Directory.Exists(OUTPUT_PATH))
            {
                Directory.CreateDirectory(OUTPUT_PATH);
            }

            //如果有先删除
            if (File.Exists($"{OUTPUT_PATH}/{MClass}Adapter.cs"))
            {
                File.Delete($"{OUTPUT_PATH}/{MClass}Adapter.cs");
                if (File.Exists($"{OUTPUT_PATH}/{MClass}Adapter.cs.meta"))
                {
                    File.Delete($"{OUTPUT_PATH}/{MClass}Adapter.cs.meta");
                }
                AssetDatabase.Refresh();
            }

            //生成适配器
            FileStream stream =
                new FileStream($"{OUTPUT_PATH}/{MClass}Adapter.cs", FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(stream);
            Stopwatch watch = new Stopwatch();
            sw.WriteLine(
                ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(t, "Adapter"));
            watch.Stop();
            Log.Msg($"Generated {OUTPUT_PATH}/{MClass}Adapter.cs in: " +
                    watch.ElapsedMilliseconds + " ms.");
            sw.Dispose();
            AssetDatabase.Refresh();
        }

        private void OnChangeAssembly()
        {
            AllClass = AssemblyManager.GetTypeList(_assembly);
        }
    }
}