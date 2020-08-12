using System;
using System.IO;
using Framework.Asynchronous;
using UnityEngine;

namespace Framework.Net
{
    public abstract class FileDownloaderBase : IFileDownloader
    {
        private Uri _baseUri;
        private int _maxTaskCount;
        
        public FileDownloaderBase(Uri baseUri, int? maxTaskCount = null)
        {
            this.BaseUri = baseUri;
            this.MaxTaskCount = maxTaskCount ?? SystemInfo.processorCount * 2;
        }

        public virtual Uri BaseUri
        {
            get => this._baseUri;
            set
            {
                if (value != null && !this.IsAllowedAbsoluteUri(value))
                    throw new NotSupportedException($"Invalid uri:{(value.OriginalString)}");

                this._baseUri = value;
            }
        }

        public virtual int MaxTaskCount
        {
            get => this._maxTaskCount;
            set => this._maxTaskCount = Mathf.Max(value > 0 ? value : SystemInfo.processorCount * 2, 1);
        }

        protected virtual bool IsAllowedAbsoluteUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
                return false;

            if ("http".Equals(uri.Scheme) || "https".Equals(uri.Scheme) || "ftp".Equals(uri.Scheme))
                return true;

            if ("file".Equals(uri.Scheme) && uri.OriginalString.IndexOf("jar:", StringComparison.Ordinal) < 0)
                return true;

            return false;
        }

        protected virtual Uri GetAbsoluteUri(Uri relativePath)
        {
            if (this._baseUri == null || this.IsAllowedAbsoluteUri(relativePath))
                return relativePath;

            return new Uri(this._baseUri, relativePath);
        }

        public virtual IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, string fileName)
        {
            return DownloadFileAsync(path, new FileInfo(fileName));
        }

        public abstract IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(Uri path, FileInfo fileInfo);

        public abstract IProgressResult<ProgressInfo, ResourceInfo[]> DownloadFileAsync(ResourceInfo[] infos);
    }
}
