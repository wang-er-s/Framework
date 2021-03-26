using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace Framework.Editor
{
    [Serializable]
    public class ILRuntimeAdapterGenerator
    {
        private string[] AllAssembly;
        private Type[] AllClass;
        private string OUTPUT_PATH;

        public void Init()
        {
            OUTPUT_PATH = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig.AdaptorPath;
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

        private string _dllPath;

        [ShowInInspector]
        [FilePath]
        [LabelText("Dll地址")]
        [InfoBox("如果上面没有，可手动选择dll")]
        public string DllPath
        {
            get => _dllPath;
            set
            {
                _dllPath = value;
                OnChangeAssembly(true);
            }
        }

        private AssemblyDefinitionAsset _assemblyDefinitionAsset;

        [LabelText("CustomDll")]
        [ShowInInspector]
        public AssemblyDefinitionAsset AssemblyDefinition
        {
            get => _assemblyDefinitionAsset;
            set
            {
                _assemblyDefinitionAsset = value;
                OnChangeAssembly(_assemblyDefinitionAsset);
            }
        }

        [HorizontalGroup()]
        [ShowInInspector]
        [LabelText("类名")] 
        [ValueDropdown("AllClass")]
        public Type MClass;

        [HorizontalGroup()]
        [Button(ButtonSizes.Large)]
        private void Add()
        {
            Classes.Add(MClass);
        }
        
        [ShowInInspector]
        [LabelText("所有需要生成的类")]
        [ListDrawerSettings(Expanded = true, HideAddButton = true)]
        public List<Type> Classes = new List<Type>();

        [Button("生成适配器",ButtonSizes.Large)]
        private void GenAdapter()
        {
            if(!Classes.Contains(MClass))
                Classes.Add(MClass);
            foreach (var type in Classes)
            {
                var _type = type;
                GenOneAdapter(_type);
            }
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

        private void OnChangeAssembly(bool customDllPath = false)
        {
            if (customDllPath)
            {
                var dll = System.Reflection.Assembly.Load(File.ReadAllBytes(DllPath));
                AllClass = dll.GetTypes();
            }
            else
            {
                AllClass = AssemblyManager.GetTypeList(_assembly);
            }
        }

        private void OnChangeAssembly(AssemblyDefinitionAsset assemblyDefinitionAsset)
        {
            var dll = System.Reflection.Assembly.Load(assemblyDefinitionAsset.bytes);
            AllClass = dll.GetTypes();
        }
    }
}