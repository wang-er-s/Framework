using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public enum CommandArgsName
    {
        DEBUG,
        UPVERSION,
        PLATFORM,
        USEHOTFIX,
        SERVER_PATH,
        BUILDPATH,
        IncrementalBuild,
        ResOffline,
    }

    public class Build : OdinEditorWindow
    {
        [MenuItem("Framework/打包")]
        public static void Open()
        {
            var window = GetWindow<Build>();
            window.Show();
        }

        private string[] buildTarget = new[] { "Android", "StandaloneWindows", "iOS" };
        [LabelText("平台")]
        [ValueDropdown("buildTarget")]
        public string Platform = "Android";
        [LabelText("是否升级版本")]
        public bool IsUpVersion;
        [LabelText("是否使用热更")]
        public bool UseHotfix;

        private bool debugMode = true;

        [LabelText("Debug模式")]
        [ShowInInspector]
        [InfoBox("Debug模式会生成pdb，且开启Development")]
        public bool DebugMode
        {
            get => debugMode;
            set
            {
                debugMode = value;
                if (debugMode)
                {
                    exportAab = false;
                }
            }
        }

        private bool exportAab;
        [LabelText("是否到处AAB(仅限安卓)")]
        [ShowInInspector]
        public bool ExportAab
        {
            get => exportAab;
            set
            {
                exportAab = value;
                if (exportAab)
                {
                    DebugMode = false;
                }
            }
        }
        [LabelText("增量ab打包")]
        public bool Incremental;
        [LabelText("资源离线")]
        public bool ResOffline = true;

        [Button(ButtonSizes.Large, Name = "打包")]
        private void BuildPlatform()
        {
            var target = (BuildTarget) Enum.Parse(typeof(BuildTarget), Platform);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildPipeline.GetBuildTargetGroup(target), target);
            var buildContext = new BuildContext(target, ResOffline, DebugMode, UseHotfix, Incremental,
                "Build", IsUpVersion, ExportAab);
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new BuildIlrNode(),
                new BuildAbNode(),
                new BuildPlayerNode()
            };
            foreach (var buildTask in buildTasks)
            {
                var result = buildTask.Run(buildContext);
                if (!string.IsNullOrEmpty(result))
                {
                    Debug.LogError(result);
                    break;
                }
            }
        }

        [Button(ButtonSizes.Large, Name = "编译Dll并打包AB")]
        private void BuildAb()
        {
            try
            {
                var target = (BuildTarget) Enum.Parse(typeof(BuildTarget), Platform);
                var context = new BuildContext(target, buildPath: "../../share/build",
                    incrementalBuild: Incremental);
                List<IBuildTask> buildTasks = new List<IBuildTask>()
                {
                    new BuildIlrNode(),
                    new BuildAbNode(),
                };
                foreach (var buildTask in buildTasks)
                {
                    var result = buildTask.Run(context);
                    if (!string.IsNullOrEmpty(result))
                    {
                        throw new BuildException(result);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        #region Jenkins

        public static void JenkinsBuildCode()
        {
            var result = new BuildIlrNode().Run(new BuildContext(BuildTarget.StandaloneWindows));
            if (!string.IsNullOrEmpty(result))
            {
                throw new BuildException(result);
            }
        }
        
        public static void JenkinsBuildCodeAndAb()
        {
            BuildTarget buildTarget =
                (BuildTarget) Enum.Parse(typeof(BuildTarget), GetEnvironmentVariable(CommandArgsName.PLATFORM));
            var outPath = GetEnvironmentVariable(CommandArgsName.BUILDPATH);
            var incremental = bool.Parse(GetEnvironmentVariable(CommandArgsName.IncrementalBuild));
            var context = new BuildContext(buildTarget, buildPath: outPath, incrementalBuild: incremental);
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new BuildIlrNode(),
                new BuildAbNode(),
            };
            foreach (var buildTask in buildTasks)
            {
                var result = buildTask.Run(context);
                if (!string.IsNullOrEmpty(result))
                {
                    throw new BuildException(result);
                }
            }
        }

        public static void JenkinsBuildAll()
        {
            BuildTarget buildTarget =
                (BuildTarget) Enum.Parse(typeof(BuildTarget), GetEnvironmentVariable(CommandArgsName.PLATFORM));
            bool debug = Boolean.Parse(GetEnvironmentVariable(CommandArgsName.DEBUG));
            bool useHotfix = Boolean.Parse(GetEnvironmentVariable(CommandArgsName.USEHOTFIX));
            var outPath = GetEnvironmentVariable(CommandArgsName.BUILDPATH);
            var incremental = bool.Parse(GetEnvironmentVariable(CommandArgsName.IncrementalBuild));
            var resOffline = bool.Parse(GetEnvironmentVariable(CommandArgsName.ResOffline));
            var buildContext = new BuildContext(buildTarget, resOffline, debug, useHotfix, incremental, outPath, false,
                false);
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new BuildIlrNode(),
                new BuildAbNode(),
                new BuildPlayerNode()
            };
            foreach (var buildTask in buildTasks)
            {
                var result = buildTask.Run(buildContext);
                if (!string.IsNullOrEmpty(result))
                {
                    throw new BuildException(result);
                }
            }
        }

        static string GetEnvironmentVariable(CommandArgsName commandArgsName)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.Contains(commandArgsName.ToString()))
                {
                    string result = arg.Split(':')[1];
                    return result;
                }
            }
            Log.Warning($"传入参数出错，没有找到 --{commandArgsName}");
            return String.Empty;

        }

        #endregion

#if UNITY_IPHONE
        private static string AdmobIosKey = "ca-app-pub-5970272688238017~1417338235";

        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                // Read.
                string projectPath = PBXProject.GetPBXProjectPath(path);
                PBXProject project = new PBXProject();
                project.ReadFromString(File.ReadAllText(projectPath));
                string targetGUID = project.GetUnityFrameworkTargetGuid();
                AddFrameworks(project, targetGUID);
                string plistPath = Path.Combine(path, "Info.plist");
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;

                // Change value of CFBundleVersion in Xcode plist
                var buildKey = "GADApplicationIdentifier";
                rootDict.SetString(buildKey, AdmobIosKey);
                var capKey = "UIRequiredDeviceCapabilities";
                if (rootDict.values.ContainsKey(capKey))
                    rootDict.values.Remove(capKey);
                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());

                // Write.
                File.WriteAllText(projectPath, project.WriteToString());
                System.Diagnostics.Process.Start("pod", $"install --project-directory={path}");
            }
        }

        static void AddFrameworks(PBXProject project, string targetGUID)
        {
            // Frameworks 
            project.AddFrameworkToProject(targetGUID, "libz.dylib", false);
            project.AddFrameworkToProject(targetGUID, "libsqlite3.tbd", false);
            project.AddFrameworkToProject(targetGUID, "CoreTelephony.framework", false);
        }
#endif

        private class BuildException : Exception
        {
            public BuildException(string message) : base(message)
            {
            }
        }
    }
}