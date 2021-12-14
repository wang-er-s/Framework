using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Framework.Helper
{
    public static class FileHelper
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

        public static void CopyFileStreaming(string from, string to)
        {
            byte[] buffer = new byte[32 * 1024];
            using (FileStream fromFile = new FileStream(from, FileMode.Open, FileAccess.Read))
            {
                using (FileStream toFile =
                    new FileStream(to, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    int bytesRead;
                    while ((bytesRead = fromFile.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        toFile.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
        
        public static string GetRelativePath(string currentPath, string fromDirectory)
        {
            if (fromDirectory == null)
                throw new ArgumentNullException(nameof(fromDirectory));

            if (currentPath == null)
                throw new ArgumentNullException(nameof(currentPath));

            if (!currentPath.Contains(fromDirectory))
                throw new Exception($"路径出错 {currentPath}   {fromDirectory}");

            var result = currentPath.RemoveString(fromDirectory);
            if (result.StartsWith("/") || result.StartsWith("\\"))
            {
                result = result.Remove(0,1);
            }
            return result;
        }
    }
}