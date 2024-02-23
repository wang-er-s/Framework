using System.IO;
using UnityEngine;

namespace Framework
{
    public class YooBundleStream : FileStream
    {
        private string bundleName;
        private int bundleNameLength;
        private bool hasDecrypt = false;
        private int totalReadBytes = 0;
        public YooBundleStream(string path, string bundleName,FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share)
        {
            this.bundleName = bundleName;
            bundleNameLength = bundleName.Length;
        }
        
        public override int Read(byte[] array, int offset, int count)
        {
            var index = base.Read(array, offset, count);
            if (hasDecrypt) return index;
            int startIndex = totalReadBytes;
            var encryptOffset = CalcEncryptOffset(bundleNameLength) - startIndex;
            if (encryptOffset > 0)
            {
                for (int i = 0; i < encryptOffset; i++)
                {
                    array[i] ^= (byte)(bundleName[(startIndex + i) % bundleNameLength] + startIndex + i);
                }

                if (encryptOffset >= CalcEncryptOffset(bundleNameLength))
                {
                    hasDecrypt = true;
                }
            }
            totalReadBytes += count;
            return index;
        }

        public static int CalcEncryptOffset(int length)
        {
            return Mathf.Max(length, 100);
        }
    }
}