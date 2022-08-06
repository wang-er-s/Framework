using Framework;
using Framework.Asynchronous;
using Framework.Editor;
using UnityEditor;

public class BuildIlrNode : IBuildTask
{
    public string Run(BuildContext context)
    {
        AsyncResult result = new AsyncResult();
#if ILRUNTIME
        var config = ConfigBase.Load<FrameworkRuntimeConfig>();
        config.ILRConfig.UsePbd = context.Debug;
        config.ILRConfig.UseHotFix = context.UseHotfix;
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        if (context.UseHotfix)
            ILRuntimeCLRBinding.GenerateCLRBindingByAnalysis(false);
        return ILRuntimeBuildDll.ReleaseBuild();
#endif
        return string.Empty;
    }
}