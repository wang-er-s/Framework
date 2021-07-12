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
using Framework.Asynchronous;

namespace Framework.Net
{
    public interface IFileDownloader
    {
        IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(string path, string fileName);

        IProgressResult<ProgressInfo, FileInfo> DownloadFileAsync(string path, FileInfo fileInfo);

        IProgressResult<ProgressInfo, ResourceInfo[]> DownloadFileAsync(ResourceInfo[] infos);
    }

    public class ResourceInfo
    {
        public ResourceInfo(string path, FileInfo fileInfo, long fileSize = -1)
        {
            this.Path = path;
            this.FileInfo = fileInfo;
            this.FileSize = fileSize;
        }
        
        public ResourceInfo(string path, string filePath, long fileSize = -1)
        {
            this.Path = path;
            this.FileInfo = new FileInfo(filePath);
            this.FileSize = fileSize;
        }

        public string Path { get; private set; }

        public FileInfo FileInfo { get; private set; }

        public long FileSize { get; set; }
    }
}
