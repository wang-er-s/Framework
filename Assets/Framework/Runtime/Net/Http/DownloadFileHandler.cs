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

using System.IO;
using UnityEngine.Networking;

namespace Framework.Net
{
    public class DownloadFileHandler : DownloadHandlerScript
    {
        private int _totalSize = -1;
        private int _completedSize = 0;
        private readonly FileInfo _fileInfo;
        private readonly FileInfo _tmpFileInfo;
        private FileStream _fileStream;

        public DownloadFileHandler(string fileName) : this(new FileInfo(fileName))
        {
        }

        public DownloadFileHandler(FileInfo fileInfo) : base(new byte[8192])
        {
            this._fileInfo = fileInfo;
            this._tmpFileInfo = new FileInfo(this._fileInfo.FullName + ".tmp");
            if (this._tmpFileInfo.Exists)
                _tmpFileInfo.Delete();

            if (_tmpFileInfo.Directory != null && !_tmpFileInfo.Directory.Exists)
                _tmpFileInfo.Directory.Create();

            this._fileStream = _tmpFileInfo.Create();
        }

        protected override byte[] GetData()
        {
            return null;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || data.Length < 1)
                return false;

            _fileStream.Write(data, 0, dataLength);
            _fileStream.Flush();
            _completedSize += dataLength;
            return true;
        }

        protected override float GetProgress()
        {
            if (_totalSize <= 0)
                return 0;
            return (float) _completedSize / _totalSize;
        }

        protected override void CompleteContent()
        {
            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }

            if (_fileInfo.Exists)
                _fileInfo.Delete();

            _tmpFileInfo.MoveTo(_fileInfo.FullName);
        }

        protected override void ReceiveContentLengthHeader(ulong contentLength)
        {
            this._totalSize = (int) contentLength;
        }

        ~DownloadFileHandler()
        {
            if (_fileStream == null) return;
            _fileStream.Dispose();
            _fileStream = null;
        }
    }
}
