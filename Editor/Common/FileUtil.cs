using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class FileUtil
{
    private static bool IsLinkDir(string path, out string linkDir)
    {
        linkDir = path;
        string relativeRoot = Path.GetRelativePath(Application.dataPath, path);
        if (relativeRoot.StartsWith(".") || relativeRoot.StartsWith(".."))
        {
            return false;
        }

        FileSystemInfo f = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
        if (f.Exists)
        {
            if (f.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                return true;
            }
        }
        else
        {
            return false;
        }

        return IsLinkDir(Path.GetDirectoryName(path), out linkDir);
    }

    public static bool GetLinkSourceDir(string path, out string linkSourceDir)
    {
        linkSourceDir = string.Empty;
        if (IsLinkDir(path, out var linkDir))
        {
            var result = ShellUtils.ExecuteCommand("cmd.exe", $"/c dir {Path.GetFullPath(Path.GetDirectoryName(linkDir))}");
            foreach (var line in result.Output.Split('\n'))
            {
                var match = Regex.Match(line, @$"<JUNCTION>\s+{Path.GetFileName(linkDir)}\s+\[(.*?)\]");
                if (match.Success)
                {
                    var relativePath = Path.GetRelativePath(linkDir, path);
                    linkSourceDir = Path.Combine(match.Groups[1].Value, relativePath);
                    return true;
                }
            }
        }

        return false;
    }
}