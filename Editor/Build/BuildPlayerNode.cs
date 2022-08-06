using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Framework;
using Framework.Asynchronous;
using Framework.Editor;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using IAsyncResult = Framework.Asynchronous.IAsyncResult;

public class BuildPlayerNode : IBuildTask
{
    public string Run(BuildContext context)
    {
        if (context.UpVersion)
        {
            UpVersion(context.BuildTarget);
        }
        var buildPath = Path.Combine(context.BuildPath, FApplication.GetPlatformPath(context.BuildTarget));
        Directory.CreateDirectory(buildPath);
        switch (context.BuildTarget)
        {
            case BuildTarget.Android:
                var buildResult = BuildAndroid(context.Debug, buildPath, context.ExportAAB);
                if (!string.IsNullOrEmpty(buildResult))
                {
                    return buildResult;
                }
                break;
            case BuildTarget.iOS:
                throw new Exception("IOS打包还没写😃");
            case BuildTarget.StandaloneWindows:
                var buildResult2 = BuildWin(buildPath, context.Debug);
                if (!string.IsNullOrEmpty(buildResult2))
                {
                    return buildResult2;
                }
                break;
        }
        EditorUtility.OpenWithDefaultApp(buildPath);
        return String.Empty;
    }

    private void UpVersion(BuildTarget buildTarget)
    {
        var bundleVersion = PlayerSettings.bundleVersion;
        string[] strs = bundleVersion.Split('.');
        var lastVersion = int.Parse(strs[strs.Length - 1]) + 1;
        strs[strs.Length - 1] = lastVersion.ToString();
        PlayerSettings.bundleVersion = string.Join(".", strs);
        if (buildTarget == BuildTarget.Android)
            PlayerSettings.Android.bundleVersionCode += 1;
        else if (buildTarget == BuildTarget.iOS)
            PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
        AssetDatabase.SaveAssets();
    }

    private string BuildAndroid(bool isDebug, string path, bool exportAab)
    {
        var config = ConfigBase.Load<FrameworkEditorConfig>();
        var keyStoreName = config.Android.KeystoreName;
        var keyStorePwd = config.Android.KeystorePwd;
        var keyAliasName = config.Android.KeyAliasName;
        var keyAliasPwd = config.Android.KeyAliasPwd;
        if (isDebug)
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            EditorUserBuildSettings.buildAppBundle = false;
        }
        else
        {
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
            EditorUserBuildSettings.buildAppBundle = exportAab;
        }
        EditorUserBuildSettings.development = isDebug;
        EditorUserBuildSettings.buildWithDeepProfilingSupport = false;
        AssetDatabase.SaveAssets();
        PlayerSettings.keystorePass = keyStorePwd;
        PlayerSettings.keyaliasPass = keyAliasPwd;
        PlayerSettings.Android.keystoreName = keyStoreName;
        PlayerSettings.Android.keyaliasName = keyAliasName;
        AssetDatabase.SaveAssets();
        BuildReport build = null;
        var apkName = Application.productName + ".apk";
        if (isDebug)
        {
            build = BuildPipeline.BuildPlayer(GetBuildScenes(), Path.Combine(path, apkName),
                BuildTarget.Android, BuildOptions.Development);
        }
        else
        {
            build = BuildPipeline.BuildPlayer(GetBuildScenes(), Path.Combine(path, apkName),
                BuildTarget.Android, BuildOptions.None);
        }
        return CheckBuildResult(build);
    }

    private string BuildWin(string path, bool debug, bool withVersion = false)
    {
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.Mono2x);
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1080;
        PlayerSettings.defaultScreenHeight = 1920;
        PlayerSettings.resizableWindow = true;
        
        if (withVersion)
            path += ("_" + Application.version);
        BuildReport build = null;
        if (debug)
        {
            build = BuildPipeline.BuildPlayer(GetBuildScenes(), Path.Combine(path, "run.exe"),
                BuildTarget.StandaloneWindows, BuildOptions.Development);
        }
        else
        {
            build = BuildPipeline.BuildPlayer(GetBuildScenes(), Path.Combine(path, "run.exe"),
                BuildTarget.StandaloneWindows, BuildOptions.None);
        }
        return CheckBuildResult(build);
    }

    string[] GetBuildScenes()
    {
        List<string> names = new List<string>();
        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        if (names.Count <= 0)
            Log.Error("EditorBuildSettings中没有找到场景");
        foreach (var sceneName in names)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                Log.Error("Build Settings中的场景有误，检查一下");
                break;
            }
        }
        return names.ToArray();
    }

    private string CheckBuildResult(BuildReport build)
    {
        StringBuilder error = new StringBuilder();
        foreach (var buildStep in build.steps)
        {
            foreach (var buildStepMessage in buildStep.messages)
            {
                if (buildStepMessage.type == LogType.Error)
                {
                    error.AppendLine(buildStepMessage.content);
                }
            }
        }
        if (!string.IsNullOrEmpty(error.ToString()))
        {
            Debug.LogError(error.ToString());
        }
        if (build.summary.result != BuildResult.Succeeded)
        {
            return error.ToString();
        }
        else
        {
            return string.Empty;
        }
    }
}