using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

namespace Framework.Util
{
    public static class FileUtils
    {
        //默认文件目录根
        public static string DirRoot { get; private set; }

        //热更新的目录，拥有资源加载的更高优先级
        public static string DirRoot2Path { get; private set; }
        
        public static void Init(string path, string pathSenior, string encrypt, string encryptHead)
        {
            DirRoot = path;
            DirRoot2Path = pathSenior;
        }
        
        public static bool IsFileFullPathSenior(string filename)
        {
            string fileFullPath = Path.Combine(DirRoot2Path, filename);
            if (File.Exists(fileFullPath))
                return true;
            return false;
        }
        
        public static string GetFileReadPath(string filename, bool senior)
        {
            if (senior)
                return Path.Combine(DirRoot2Path, filename);
            else
                return Path.Combine(DirRoot, filename);
        }
        
        public static string CreateDirectory(string relativePath, bool delExist = false)
        {
            string fullDirPath = Path.Combine(DirRoot2Path, relativePath);
            if (Directory.Exists(fullDirPath))
            {
                if (delExist)
                    Directory.Delete(fullDirPath, true);
                else
                    return fullDirPath;
            }

            Directory.CreateDirectory(fullDirPath);
            return fullDirPath;
        }

        public static string GetFileWriteFullPath(string filename)
        {
            return Path.Combine(DirRoot2Path, filename);
        }
        
        public static string GetFileReadFullPath(string filename, bool checkInside = true)
        {
            string fileFullPath = Path.Combine(DirRoot2Path, filename);
            if (File.Exists(fileFullPath))
                return fileFullPath;
            if (checkInside)
            {
                return Path.Combine(DirRoot, filename);
            }

            return null;
        }
        
        /// <summary>
        /// 文件打开，对于包内文件只用于非压缩格式的
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Stream OpenFile(string filePath)
        {
            Stream stream = null;
            if (filePath.Contains("://"))
            {
                using (UnityWebRequest www = UnityWebRequest.Get(filePath))
                {
                    www.SendWebRequest();
                    while (!www.isDone) ;
                    if (null != www.error)
                    {
                        Log.Msg($"open file err : {www.error}");
                        throw new Exception(filePath);
                    }
                    else
                        stream = new MemoryStream(www.downloadHandler.data);
                }
            }
            else
            {
                if (File.Exists(filePath))
                    stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            return stream;
        }
        
        #if UNITY_ANDROID
        private static Lazy<AndroidJavaClass> _AndroidAssetUtilsClass =
	        new Lazy<AndroidJavaClass>(() => new AndroidJavaClass("com.parallel.android.utils.AssetUtils")
	        );
        private static bool IsAndroidAssetExist(string assetPath)
        {
	        return _AndroidAssetUtilsClass.Value.CallStatic<bool>("isAssetExists", assetPath);
        }
#endif

		public static bool IsFileExist(string filePath)
		{
#if UNITY_ANDROID
			if (filePath.Contains(".apk!/assets/"))
			{
				var assetPath = Regex.Replace(filePath, "^.+\\.apk!/assets/", "");
				return IsAndroidAssetExist(assetPath);
			}
#endif

			if (filePath.Contains("://"))
			{
				using (UnityWebRequest www = UnityWebRequest.Get(filePath))
				{
					www.SendWebRequest();
					while (!www.isDone) ;
					return www.error == null;
				}
			}
			else
			{
				return File.Exists(filePath);
			}
		}

		public static byte[] GetFileBytes(string filePath)
		{
			try
			{
				if (filePath.Contains("://"))
				{
					using (UnityWebRequest www = UnityWebRequest.Get(filePath))
					{
						www.SendWebRequest();
						while (!www.isDone) ;
						if (null != www.error)
							throw new Exception(filePath);
						else
							return www.downloadHandler.data;
					}
				}
				else
				{
					using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
					{
						byte[] bytes = new byte[fs.Length];
						fs.Read(bytes, 0, bytes.Length);
						return bytes;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error($"GetFileBytes err : {ex.Message}");
				return null;
			}
		}

		public static void WriteFile(string filePath, byte[] data)
		{
			using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				using (MemoryStream stream = new MemoryStream(data))
				{
					CopyStream(stream, fs);
				}
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			int bufferSize = 2048;
			bool isSuccessed = false;
			try
			{
				byte[] buffer = new byte[bufferSize];
				while (true)
				{
					int read = input.Read(buffer, 0, bufferSize);
					if (read <= 0)
					{
						isSuccessed = true;
						break;
					}

					output.Write(buffer, 0, read);
				}
			}
			catch (Exception ex)
			{
				Log.Msg($"fail to copy stream: {ex.Message}");
			}

			if (isSuccessed)
				output.Flush();
		}

		public static byte[] ReadAllBytes(this Stream stream)
		{
			if (stream is MemoryStream)
				return ((MemoryStream) stream).ToArray();
			using (MemoryStream ms = new MemoryStream())
			{
				CopyStream(stream, ms);
				return ms.ToArray();
			}
		}

		public static long GetFileSize(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
				{
					FileInfo info = new FileInfo(fileName);
					return info.Length;
				}
			}
			catch
			{
				return -1;
			}

			return 0;
		}

		public static string GetFileMd5(string fileName)
		{
			try
			{
				byte[] fileBuffer = File.ReadAllBytes(fileName);
				MD5 md5Hash = MD5.Create();
				byte[] result = md5Hash.ComputeHash(fileBuffer);
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < result.Length; i++)
				{
					sb.AppendFormat("{0:x2}", result[i]);
				}

				return sb.ToString();
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public static bool RemoveFile(string fileName)
		{
			try
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error($"fail del file {fileName}:{ex.Message}");
				return false;
			}
		}

		public static bool RemoveDir(string dir)
		{
			try
			{
				if (Directory.Exists(dir))
					Directory.Delete(dir, true);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error($"fail del dir {dir}:{ex.Message}");
				return false;
			}
		}

		public static bool SaveStrToFile(String str, string fileName)
		{
			return SaveStrToPath(str, GetFileWriteFullPath(fileName));
		}

		public static bool SaveBytesToFile(byte[] data, string fileName)
		{
			return SaveBytesToPath(data, GetFileWriteFullPath(fileName));
		}

		public static bool SaveStrToPath(String str, string path)
		{
			return SaveBytesToPath(Encoding.UTF8.GetBytes(str), path);
		}

		public static bool SaveBytesToPath(byte[] data, string path)
		{
			if (null == path)
				return false;
			try
			{
				using (FileStream output = new FileStream(path, FileMode.Create))
				{
					MemoryStream ms = new MemoryStream(data);
					CopyStream(ms, output);
					return true;
				}
			}
			catch (Exception ex)
			{
				Log.Error($"SaveBytesToPath {path} err:{ex.Message}");
				return false;
			}
		}

		public static string GetStrFromFile(string fileName, bool checkInside = true)
		{
			var path = FileUtils.GetFileReadFullPath(fileName, checkInside);
			return GetStrFromPath(path);
		}

		public static string GetStrFromPath(string path)
		{
			if (null == path)
				return null;
			try
			{
				using (var stream = FileUtils.OpenFile(path))
				{
					if (null != stream)
					{
						using (StreamReader sr = new StreamReader(stream))
						{
							return sr.ReadToEnd();
						}

					}
					else
						return null;
				}
			}
			catch (Exception ex)
			{
				Log.Warning($"GetStrFromPath {path} err:{ex.Message}");
				return null;
			}
		}
		
		public static List<string> GetFiles(string dirPath, string patterns,bool ignoreSpec = true, bool ignoreMeta = true)
		{
			string fullDirPath = Path.GetFullPath(dirPath);
			string[] pats = null;
			if (!string.IsNullOrEmpty(patterns))
				pats = patterns.Split(new char[] {'|'});
			List<FileInfo> files = GetFiles(dirPath, pats, ignoreSpec, ignoreMeta);
			List<string> ret = new List<string>();
			int startIndex = fullDirPath.Length;
			foreach (var file in files)
			{
				string assetFile = dirPath + file.FullName.Substring(startIndex);
				assetFile = assetFile.Replace('\\', '/');
				ret.Add(assetFile);
			}
			return ret;
		}
		
		public static List<FileInfo> GetFiles(string dirPath,string[] patterns = null,bool ignoreSpec = true, bool ignoreMeta = true)
		{
			DirectoryInfo directory = new DirectoryInfo(dirPath);
			List<FileInfo> ret = new List<FileInfo>();
			foreach(FileInfo f in directory.EnumerateFiles("*.*",SearchOption.AllDirectories))
			{
				if (ignoreSpec)
				{
					if (f.FullName.Contains("/_") || f.FullName.Contains("\\_"))
						continue;
				}

				if(ignoreMeta && f.Name.EndsWith(".meta",StringComparison.OrdinalIgnoreCase))
					continue;
				if(null == patterns)
					ret.Add(f);
				else
				{
					foreach(string pattern in patterns)
					{
						if(f.Name.EndsWith(pattern,StringComparison.OrdinalIgnoreCase))
							ret.Add(f);
					}
				}
			}
			return ret;
		}
		
		public static void CreateDir(string dirPath, bool reIfExist)
		{
			if (reIfExist)
			{
				if (Directory.Exists(dirPath))
					Directory.Delete(dirPath, true);
				Directory.CreateDirectory(dirPath);
			}
			else
			{
				if (!Directory.Exists(dirPath))
					Directory.CreateDirectory(dirPath);
			}
		}

    }
}