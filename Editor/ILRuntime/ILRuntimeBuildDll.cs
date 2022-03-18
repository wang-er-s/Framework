using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

#if ILRUNTIME
namespace Framework.Editor
{
    [Serializable]
    [LabelText("生成DLL")]
    public class ILRuntimeBuildDll
    {
        [HorizontalGroup()]
        [Button("编译dll(Roslyn-Debug)",ButtonSizes.Large)]
        public static void DebugBuild()
        {
            BuildDLL(true);
        }

        [HorizontalGroup()]
        [Button("编译dll(Roslyn-Release)",ButtonSizes.Large)]
        public  static void ReleaseBuild()
        {
            BuildDLL(false);
        }

        private static List<string> defineList = new List<string>();
        private static bool usePdb;
        private static ILRConfig ilrConfig;
        
        private static void BuildDLL(bool isDebug)
        {
            var runtimeConfig = ConfigBase.Load<FrameworkRuntimeConfig>();
            ilrConfig = runtimeConfig.ILRConfig;
            EditorUtility.SetDirty(runtimeConfig);
            AssetDatabase.SaveAssets();
            usePdb = ilrConfig.UsePbd;
            var dllName = ilrConfig.DllName;
            string codeSource = Application.dataPath + "/_scripts@hotfix";
            string outPath = ilrConfig.DllGenPath + $"/{dllName}.dll";
            List<string> allDll = new List<string>();
            var allCsFiles = new List<string>(Directory.GetFiles(codeSource, "*.cs", SearchOption.AllDirectories));
            try
            {
                EditorUtility.DisplayProgressBar("编译服务", "[1/2]查找引用和脚本...", 0.5f);
                FindDLLByCSPROJ("Assembly-CSharp.csproj", ref allDll);
                EditorUtility.DisplayProgressBar("编译服务", "[2/2]开始编译hotfix.dll...", 0.7f);
                BuildByRoslyn(allDll, allCsFiles, outPath, isDebug);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }
        
        public static void BuildDLL(string codeSource,string outPath,bool isDebug)
        {
            var runtimeConfig = ConfigBase.Load<FrameworkRuntimeConfig>();
            var config = runtimeConfig.ILRConfig;
            EditorUtility.SetDirty(runtimeConfig);
            AssetDatabase.SaveAssets();
            usePdb = config.UsePbd;
            //string codeSource = Application.dataPath + "/_scripts@hotfix";
            //string outPath = config.DllGenPath + $"/{dllName}.dll";
            List<string> allDll = new List<string>();
            var allCsFiles = new List<string>(Directory.GetFiles(codeSource, "*.cs", SearchOption.AllDirectories));
            try
            {
                EditorUtility.DisplayProgressBar("编译服务", "[1/2]查找引用和脚本...", 0.5f);
                FindDLLByCSPROJ("Assembly-CSharp.csproj", ref allDll);
                EditorUtility.DisplayProgressBar("编译服务", "[2/2]开始编译hotfix.dll...", 0.7f);
                BuildByRoslyn(allDll, allCsFiles, outPath, isDebug);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 解析project中的dll
        /// </summary>
        /// <returns></returns>
        private static void FindDLLByCSPROJ(string projName, ref List<string> dllList)
        {
            var projpath = FApplication.ProjectRoot + "/" + projName;
            XmlDocument xml = new XmlDocument();
            xml.Load(projpath);
            XmlNode ProjectNode = null;

            foreach (XmlNode x in xml.ChildNodes)
            {
                if (x.Name == "Project")
                {
                    ProjectNode = x;
                    break;
                }
            }
            defineList.Clear();
            List<string> csprojList = new List<string>();
            foreach (XmlNode childNode in ProjectNode.ChildNodes)
            {
                if (childNode.Name == "ItemGroup")
                {
                    foreach (XmlNode item in childNode.ChildNodes)
                    {
                        if (item.Name == "Reference") //DLL 引用
                        {
                            var HintPath = item.FirstChild;
                            var dir = HintPath.InnerText.Replace("/", "\\");
                            dllList.Add(dir);
                        }
                        else if (item.Name == "ProjectReference") //工程引用
                        {
                            var csproj = item.Attributes[0].Value;
                            csprojList.Add(csproj);
                        }
                    }
                }
                else if (childNode.Name == "PropertyGroup")
                {
                    foreach (XmlNode item in childNode.ChildNodes)
                    {
                        if (item.Name == "DefineConstants")
                        {
                            var define = item.InnerText;
                
                            var defines = define.Split(';');
                
                            defineList.AddRange(defines);
                        }
                
                    }
                }
            }

            // 删除UNITY_EDITOR的宏
            for (int i = 0; i < defineList.Count; i++)
            {
                if (defineList[i] == "UNITY_EDITOR")
                {
                    defineList.RemoveAt(i);
                    break;
                }
            }

            //csproj也加入
            foreach (var csproj in csprojList)
            {
                //有editor退出
                if (csproj.ToLower().Contains("editor")) continue;
                //添加扫描到的dllF
                FindDLLByCSPROJ(csproj, ref dllList);
                //
                var gendll = FApplication.Library + "/ScriptAssemblies/" + csproj.Replace(".csproj", ".dll");
                if (!File.Exists(gendll))
                {
                    Debug.LogError("不存在:" + gendll);
                }

                dllList.Add(gendll);
            }

            //去重
            dllList = dllList.Distinct().ToList();
            // 删除unity生成的热更dll  防止重复
            for (int i = 0; i < dllList.Count; i++)
            {
                if (Path.GetFileNameWithoutExtension(dllList[i]) == ilrConfig.DllName)
                {
                    dllList.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// 编译dll
        /// </summary>
        /// <param name="rootpaths"></param>
        /// <param name="output"></param>
        private static bool BuildByRoslyn(List<string> dlls, List<string> codefiles, string output, bool isDebug)
        {
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                for (int i = 0; i < dlls.Count; i++)
                {
                    dlls[i] = dlls[i].Replace("\\", "/");
                }
                for (int i = 0; i < codefiles.Count; i++)
                {
                    codefiles[i] = codefiles[i].Replace("\\", "/");
                }
                output = output.Replace("\\", "/");
            }
            
            //添加语法树
            List<Microsoft.CodeAnalysis.SyntaxTree> codes = new List<Microsoft.CodeAnalysis.SyntaxTree>();
            var opa = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: defineList);
            foreach (var cs in codefiles)
            {
                //判断文件是否存在
                if (!File.Exists(cs)) continue;
                //
                var content = File.ReadAllText(cs);
                var syntaxTree = CSharpSyntaxTree.ParseText(content, opa, cs, Encoding.UTF8);
                codes.Add(syntaxTree);
            }

            //添加dll
            List<MetadataReference> assemblies = new List<MetadataReference>();
            foreach (var dll in dlls)
            {
                var metaref = MetadataReference.CreateFromFile(dll);
                if (metaref != null)
                {
                    assemblies.Add(metaref);
                }
            }

            //创建目录
            var dir = Path.GetDirectoryName(output);
            Directory.CreateDirectory(dir);
            //编译参数
            CSharpCompilationOptions option = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: isDebug ? OptimizationLevel.Debug : OptimizationLevel.Release, warningLevel: 4,
                    allowUnsafe: true);

            //创建编译器代理
            var assemblyname = Path.GetFileNameWithoutExtension(output);
            var compilation = CSharpCompilation.Create(assemblyname, codes, assemblies, option);
            EmitResult result = null;

            var pdbPath = Path.ChangeExtension(output, "pdb");
            
            var emitOptions = new EmitOptions(debugInformationFormat: DebugInformationFormat.PortablePdb,
                pdbFilePath: pdbPath);
            using (var dllStream = new MemoryStream())
            using (var pdbStream = new MemoryStream())
            {
                result = compilation.Emit(dllStream, pdbStream, options: emitOptions);
                File.WriteAllBytes(output + ".bytes", dllStream.GetBuffer());
                pdbPath += ".bytes";
                if(File.Exists(pdbPath))
                    File.Delete(pdbPath);
                if (usePdb)
                {
                    File.WriteAllBytes(pdbPath, pdbStream.GetBuffer());
                }
            }

            // 编译失败，提示
            if (!result.Success)
            {
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity ==
                    DiagnosticSeverity.Error);
                StringBuilder sb = new StringBuilder();
                foreach (var diagnostic in failures)
                {
                    sb.AppendLine(diagnostic.ToString());
                }
                throw new Exception(sb.ToString());
            }
            else
            {
                Debug.Log("编译DLL成功");
            }
            return result.Success;
        }
    }
}
#endif