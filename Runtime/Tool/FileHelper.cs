using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

            bool isRooted = (Path.IsPathRooted(fromDirectory) && Path.IsPathRooted(currentPath));

            if (isRooted)
            {
                bool isDifferentRoot = (String.Compare(Path.GetPathRoot(fromDirectory), Path.GetPathRoot(currentPath), StringComparison.OrdinalIgnoreCase) != 0);

                if (isDifferentRoot)
                    return currentPath;
            }

            List<string> relativePath = new List<string>();
            string[] fromDirectories = fromDirectory.Split(Path.DirectorySeparatorChar);

            string[] toDirectories = currentPath.Split(Path.DirectorySeparatorChar);

            int length = Math.Min(fromDirectories.Length, toDirectories.Length);

            int lastCommonRoot = -1;

            // find common root
            for (int x = 0; x < length; x++)
            {
                if (String.Compare(fromDirectories[x], toDirectories[x], StringComparison.OrdinalIgnoreCase) != 0)
                    break;

                lastCommonRoot = x;
            }

            if (lastCommonRoot == -1)
                return currentPath;

            // add relative folders in from path
            for (int x = lastCommonRoot + 1; x < fromDirectories.Length; x++)
            {
                if (fromDirectories[x].Length > 0)
                    relativePath.Add("..");
            }

            // add to folders to path
            for (int x = lastCommonRoot + 1; x < toDirectories.Length; x++)
            {
                relativePath.Add(toDirectories[x]);
            }

            // create relative path
            string[] relativeParts = new string[relativePath.Count];
            relativePath.CopyTo(relativeParts, 0);

            string newPath = string.Join(Path.DirectorySeparatorChar.ToString(), relativeParts);

            return newPath;
        }
    }
}