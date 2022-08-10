using System;
using System.IO;
using Framework;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

public class BuildAbNode : IBuildTask
{
    public string Run(BuildContext context)
    {
        AssetBundleBuilderSettingData.Setting.BuildVersion++;
        AssetBundleBuilderSettingData.SaveFile();

        YooAssetSettingsData.Setting.PlayMode = YooAssets.EPlayMode.HostPlayMode;
        YooAssetSettingsData.Save();

        // 命令行参数
        int buildVersion = AssetBundleBuilderSettingData.Setting.BuildVersion;

        // 构建参数
        string defaultOutputRoot = AssetBundleBuilderHelper.GetDefaultOutputRoot();
        defaultOutputRoot = $"{context.BuildPath}/CDN/Cache";
        BuildParameters buildParameters = new BuildParameters();
        buildParameters.OutputRoot = defaultOutputRoot;
        buildParameters.BuildTarget = context.BuildTarget;
        buildParameters.BuildPipeline = EBuildPipeline.BuiltinBuildPipeline;
        buildParameters.BuildMode = context.IncrementalBuild ? EBuildMode.IncrementalBuild : EBuildMode.ForceRebuild;
        buildParameters.BuildVersion = buildVersion;
        buildParameters.BuildinTags = AssetBundleCollectorSettingData.GetAllTags();
        buildParameters.VerifyBuildingResult = true;
        buildParameters.EnableAddressable = false;
        buildParameters.CopyBuildinTagFiles = !context.IncrementalBuild;
        buildParameters.CompressOption = ECompressOption.LZ4;
        buildParameters.OutputNameStyle = EOutputNameStyle.BundleName_HashName;
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
                $"{buildParameters.OutputRoot}/{buildParameters.BuildTarget}/{buildParameters.BuildVersion}";
            DirectoryInfo buildOutDir = new DirectoryInfo(buildOutPath);
            buildOutDir.CopyTo(cdnPath);
            Debug.Log($"拷贝到{cdnPath}成功");
            return String.Empty;
        }
    }
}