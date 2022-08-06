using System.IO;
using Framework;
using UnityEditor;

public class BuildContext
{
    public readonly bool Debug;
    public readonly bool UseHotfix;
    public readonly bool IncrementalBuild;
    public readonly string BuildPath;
    public readonly bool UpVersion;
    public readonly bool ExportAAB;
    public readonly BuildTarget BuildTarget;
    

    public BuildContext(BuildTarget buildTarget, bool debug = true, bool useHotfix = false, bool incrementalBuild = false, string buildPath = "", bool upVersion = false, bool exportAab = false)
    {
        BuildTarget = buildTarget;
        Debug = debug;
        UseHotfix = useHotfix;
        IncrementalBuild = incrementalBuild;
        BuildPath = buildPath;
        if (string.IsNullOrEmpty(buildPath))
        {
            BuildPath = Path.Combine(FApplication.ProjectRoot, "Build");
        }
        UpVersion = upVersion;
        ExportAAB = exportAab;
    }
}