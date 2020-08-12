using System;
using System.IO;
using Framework.Asynchronous;

namespace Framework.Net
{
    public interface IFileDownloader
    {
        IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, string fileName);

        IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, FileInfo fileInfo);

        IProgressResult<ProgressInfo, ResourceInfo[]> DownloadFileAsync(ResourceInfo[] infos);
    }

    public class ResourceInfo
    {
        public ResourceInfo(Uri path, FileInfo fileInfo, long fileSize = -1)
        {
            this.Path = path;
            this.FileInfo = fileInfo;
            this.FileSize = fileSize;
        }

        public Uri Path { get; private set; }

        public FileInfo FileInfo { get; private set; }

        public long FileSize { get; set; }
    }
}
