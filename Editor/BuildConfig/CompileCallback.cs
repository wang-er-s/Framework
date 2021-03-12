using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public static class CompileCallback
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptCompile()
        {
            MoveHotfixDll();
        }

        private static void MoveHotfixDll()
        {
            var ilrConfig = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
            var pdbLibraryPath = Path.Combine(Application.dataPath, $"../Library/ScriptAssemblies/{ilrConfig.DllName}.pdb");
            var dllLibraryPath = Path.Combine(Application.dataPath, $"../Library/ScriptAssemblies/{ilrConfig.DllName}.dll");

            var pdbSteamPath = Path.Combine(Application.streamingAssetsPath, $"{ilrConfig.DllName}.pdb");
            var dllSteamPath = Path.Combine(Application.streamingAssetsPath, $"{ilrConfig.DllName}.bytes");

            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            
            if (File.Exists(dllLibraryPath))
            {
                var file = File.Create(dllSteamPath);
                var data = File.ReadAllBytes(dllLibraryPath);
                file.Write(data, 0, data.Length);
                file.Dispose();
            }
            
            if (File.Exists(pdbLibraryPath))
            {
                if(File.Exists(pdbSteamPath))
                    File.Delete(pdbSteamPath);
                File.Copy(pdbLibraryPath, pdbSteamPath);
            }

            AssetDatabase.Refresh();
        }
        
    }
}