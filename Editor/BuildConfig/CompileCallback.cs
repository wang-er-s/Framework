namespace Framework.Editor
{
    public static class CompileCallback
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptCompile()
        {
            ILRuntimeAutoCompile();
        }

        private static void ILRuntimeAutoCompile()
        {
#if ILRUNTIME
            var config = ConfigBase.Load<FrameworkRuntimeConfig>().ILRConfig;
            if(!config.UseHotFix || !config.AutoCompile) return;
            ILRuntimeBuildDll.DebugBuild();
#endif
        }   
    }
}