using System;
using System.Collections.Generic;
using System.IO;

namespace Framework.Helper
{
    public class FileHelper
    {
        public static void DeepSearchDirFiles(string dirPath, Action<FileInfo> action, string pattern = "*")
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if(!dir.Exists) return;
            foreach (var fileInfo in dir.GetFiles(pattern))
            {
                action?.Invoke(fileInfo);
            }
            foreach (var directoryInfo in dir.GetDirectories())
            {
                DeepSearchDirFiles(directoryInfo.FullName, action);
            }
        }
        
        public static void DeepSearchDirFiles(string dirPath, ref List<string> fileNames, string pattern = "*")
        {
            fileNames.AddRange(Directory.GetFiles(dirPath, pattern, SearchOption.TopDirectoryOnly));
            foreach (var directory in Directory.GetDirectories(dirPath))
            {
                DeepSearchDirFiles(directory, ref fileNames, pattern);
            }
        }
    }
}