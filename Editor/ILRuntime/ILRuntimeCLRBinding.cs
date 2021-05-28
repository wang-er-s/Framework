using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Framework.Editor
{
    [Serializable]
    public class ILRuntimeCLRBinding
    {
        [Button("生成CLR绑定[不知道干嘛别点！]", ButtonSizes.Large)]
        private static void Gen()
        {
            GenerateCLRBindingByAnalysis(true);
        }

        public static void GenerateCLRBindingByAnalysis(bool showTips = true)
        {
            if(showTips && !EditorUtility.DisplayDialog("注意", "确定要生成吗", "是的", "点错")) return;
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
            using (System.IO.FileStream fs = new System.IO.FileStream($"Assets/StreamingAssets/{ilrConfig.DllName}.dll",
                System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);

                //Crossbind Adapter is needed to generate the correct binding code
                InitILRuntime(domain);
                var path = "Assets/_Scripts/ILRuntime/Generated";
                if(Directory.Exists(path))
                    Directory.Delete(path, true);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain,
                    "Assets/_Scripts/ILRuntime/Generated");
            }

            AssetDatabase.Refresh();
        }

        static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
        {
            //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
            var gameDll = AssemblyManager.GetAssembly(ConfigBase.Load<FrameworkRuntimeConfig>().GameDllName);
            var para = new object[] {domain};
            foreach (var type in gameDll.GetTypes())
            {
                ILRuntimeAdapterHelper.AddAdaptor(type);
                if (type.GetCustomAttribute(typeof(HotfixInitAttribute), false) != null)
                {
                    type.GetMethods().First(info => info.GetCustomAttribute(typeof(HotfixInitAttribute)) != null)
                        .Invoke(null, para);
                }
            }
            foreach (var type in typeof(ILRuntimeHelper).Assembly.GetTypes())
            {
                ILRuntimeAdapterHelper.AddAdaptor(type);
            }
            ILRuntimeHelper.InitializeILRuntime(domain);
        }
    }
}