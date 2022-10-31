using System;
using System.IO;
using Framework;
using UnityEditor.Build;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;
using BuildReport = UnityEditor.Build.Reporting.BuildReport;

public class BuildAbNode : IBuildTask , IPostprocessBuildWithReport
{
    public string Run(BuildContext context)
    {
        YooAssetSettingsData.Setting.PlayMode = context.ResOffline ? EPlayMode.OfflinePlayMode : EPlayMode.HostPlayMode;
        YooAssetSettingsData.Save();

        // 命令行参数
        // 构建参数
        string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultOutputRoot();
        defaultOutputRoot = $"{context.BuildPath}/CDN/Cache";
        BuildParameters buildParameters = new BuildParameters();
        buildParameters.OutputRoot = defaultOutputRoot;
        buildParameters.BuildTarget = context.BuildTarget;
        buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
        buildParameters.PackageName = "DefaultPackage";
        buildParameters.PackageVersion = "1.0.0";
        buildParameters.BuildMode = context.IncrementalBuild ? EBuildMode.IncrementalBuild : EBuildMode.ForceRebuild;
        buildParameters.VerifyBuildingResult = true;
        buildParameters.CompressOption = ECompressOption.LZ4;
        buildParameters.OutputNameStyle = EOutputNameStyle.BundleName_HashName;
        buildParameters.CopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyAll;
        // 执行构建
        AssetBundleBuilder builder = new AssetBundleBuilder();
        var result = builder.Run(buildParameters);
        if (!result.Success)
        {
            return $"{result.FailedTask}\n{result.FailedInfo}";
        }
        else
        {
            Debug.Log("开始拷贝");
            // bundle 拷贝到 cdn
            string cdnPath = Path.Combine(context.BuildPath, "CDN", FApplication.GetPlatformPath(context.BuildTarget), ConfigBase.Load<FrameworkRuntimeConfig>().GameVersion);
            var buildOutPath =
                $"{buildParameters.OutputRoot}/{buildParameters.BuildTarget}";
            DirectoryInfo buildOutDir = new DirectoryInfo(buildOutPath);
            buildOutDir.CopyTo(cdnPath);
            Debug.Log($"拷贝到{cdnPath}成功");
            return String.Empty;
        }
    }

    public int callbackOrder { get; }
    public void OnPostprocessBuild(BuildReport report)
    {
        YooAssetSettingsData.Setting.PlayMode = EPlayMode.EditorSimulateMode;
        YooAssetSettingsData.Save();
    }
}