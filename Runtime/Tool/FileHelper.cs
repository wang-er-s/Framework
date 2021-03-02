using System;
using System.IO;

namespace Framework.Helper
{
    public class FileHelper
    {
        public static void DeepSearchDirFiles(string dirPath, Action<FileInfo> action)
        {
            DirectoryInfo dir = new DirectoryInfo(dirPath);
            if(!dir.Exists) return;
            foreach (var fileInfo in dir.GetFiles())
            {
                action?.Invoke(fileInfo);
            }
            foreach (var directoryInfo in dir.GetDirectories())
            {
                DeepSearchDirFiles(directoryInfo.FullName, action);
            }
        }
    }
}