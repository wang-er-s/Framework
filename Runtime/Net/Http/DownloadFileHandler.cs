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
