/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using System;
using System.IO;
using UnityEngine;

namespace Framework
{
    public abstract class FileDownloaderBase : IFileDownloader
    {
        private int _maxTaskCount;

        public FileDownloaderBase(int? maxTaskCount = null)
        {
            this.MaxTaskCount = maxTaskCount ?? SystemInfo.processorCount * 2;
        }

        public virtual int MaxTaskCount
        {
            get => this._maxTaskCount;
            set => this._maxTaskCount = Mathf.Max(value > 0 ? value : SystemInfo.processorCount * 2, 1);
        }

        public virtual IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(string path, string fileName, float overtimeTime = 50)
        {
            if (fileName.StartsWith(FApplication.PathPrefix))
                fileName = fileName.RemoveString(FApplication.PathPrefix);
            return DownloadFileAsync(path, new FileInfo(fileName), overtimeTime);
        }

        public abstract IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(string path, FileInfo fileInfo, float overtimeTime = 50);

        public abstract IProgressResult<ProgressInfo, ResourceInfo[]> DownloadFileAsync(ResourceInfo[] infos);
    }
}