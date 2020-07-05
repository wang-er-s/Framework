using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Framework.BaseUtil;
using Framework.Util;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;	
#endif

namespace Framework
{
	public enum AssetType : int
	{
		UNKNOW = -1,
		OBJECT = 0,
		GAMEOBJ = 1,
		TEXTURE = 2,
		MATERIAL = 3,
		ANIM = 4,
		SOUND = 5,
		FONT = 6,
		MANIFEST = 7,
		SCRIPTABLE = 8,
		SHADER = 9,
		SPRITE_ATLAS = 10,
		BASETYPE_MAX = 10, //原始类型
		SCENE = 11,
		MULTI_ASSETS = 12,
		SHADER_COMPOSE = 13, //单独定义一组,与Multi_assets区别，方便使用
	}

	[Serializable]
	public class BundleInfo
	{
		public string path;
		public string mainName;
		public AssetType type; //是否不需要instante
		private static readonly char[] sep1 = new[] {':'};
		private static readonly char[] sep2 = new[] {','};
		[SerializeField] public string[] depends;

		private static readonly Type[] types =
		{
			typeof(UnityEngine.Object),
			typeof(GameObject),
			typeof(Texture),
			typeof(Material),
			typeof(AnimationClip),
			typeof(AudioClip),
			typeof(Font),
			typeof(AssetBundleManifest),
			typeof(ScriptableObject),
			typeof(Shader),
			typeof(SpriteAtlas),
		};

		public static BundleInfo ParseString(string infoStr)
		{
			if (string.IsNullOrEmpty(infoStr))
				return null;
			string[] parts = infoStr.Split(sep1);
			if (parts.Length < 3)
				return null;
			BundleInfo bi = new BundleInfo();
			bi.path = parts[0];
			string[] mains = parts[1].Split(sep2);
			bi.mainName = mains[0];
			int typeVal = 0;
			int.TryParse(mains[1], out typeVal);
			bi.type = (AssetType) typeVal;
			if (string.IsNullOrEmpty(parts[2]))
				bi.depends = null;
			else
				bi.depends = parts[2].Split(sep2);

			return bi;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(path).Append(':');
			sb.AppendFormat("{0},{1}:", mainName, (int) type);
			if (null != depends)
			{
				for (int i = 0; i < depends.Length; ++i)
				{
					if (i > 0)
						sb.Append(',');
					sb.Append(depends[i]);
				}
			}

			return sb.ToString();
		}

		public Type GetAssetType()
		{
			if (type > AssetType.BASETYPE_MAX)
				return types[0];
			return types[(int) type];
		}
	}

	public class BundleConfig
	{
		private static string rootPath;
		public static string RootPath => rootPath ?? (rootPath = "assetbundles/" + GetPlatformRootFolder() + "/");

		private static string trdPath; //提供一个第三方路径，用于调试补充

		public static string TrdPath
		{
			get => trdPath;
			set
			{
				if (value == null)
					trdPath = null;
				else
					trdPath = value + "/" + GetPlatformRootFolder() + "/";
			}
		}

		public static readonly string bundleFileExt = "";
		public static readonly string allDepFile = "bundle.info";

		private static string projectPath;

		public static string ProjectPath
		{
			get
			{
				if (null == projectPath)
				{
					projectPath = Application.dataPath;
					projectPath = projectPath.Substring(0, projectPath.Length - 6);
				}

				return projectPath;
			}
		}

		private Dictionary<string, BundleInfo> depMap = new Dictionary<string, BundleInfo>();

#if UNITY_EDITOR
		public static string GetPlatformRootFolder(BuildTarget target)
		{
			switch (target)
			{
				case BuildTarget.Android: return "Android";
				case BuildTarget.iOS: return "iOS";
				case BuildTarget.StandaloneWindows:
				case BuildTarget.StandaloneWindows64: return "Win64";
				case (BuildTarget) 3: //case BuildTarget.StandaloneOSXUniversal:
				case BuildTarget.StandaloneOSX: return "OSX";

				default: return "BundleRes";
			}
		}
#endif
		private static string GetPlatformRootFolder()
		{
#if UNITY_EDITOR
			return GetPlatformRootFolder(EditorUserBuildSettings.activeBuildTarget);
#else
			switch (Application.platform)
			{
				case RuntimePlatform.Android: return "Android";
				case RuntimePlatform.IPhonePlayer: return "iOS";
				case RuntimePlatform.WindowsEditor:
				case RuntimePlatform.WindowsPlayer: return "Win64";
				case RuntimePlatform.OSXEditor:
				case RuntimePlatform.OSXPlayer: return "OSX";
				default: return "BundleRes";
			}
#endif
		}

		#region Bundle信息

		public void Init(bool clear) //依赖信息文件
		{
			//TODO PathIdProfile不知道干嘛的 
			PathIdProfile.Ins.Load();
			if (clear)
				depMap.Clear();
			//解析bundle信息（地址，名字，类型，依赖的地址）
			string infoFile = FileUtils.GetFileReadFullPath(RootPath + allDepFile);
			Import(infoFile);
#if UNITY_EDITOR
			if (null != TrdPath)
			{
				infoFile = FileUtils.GetFileReadPath(TrdPath + allDepFile, false);
				if (File.Exists(infoFile))
					Import(infoFile);
			}
#endif
		}

		public void Import(string infoFile)
		{
			try
			{
				using (var stream = FileUtils.OpenFile(infoFile))
				{
					if (null == stream) return;
					using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
					{
						while (sr.Peek() > -1)
						{
							string line = sr.ReadLine();
							if (line.Length == 0)
								continue;
							BundleInfo bi = BundleInfo.ParseString(line);
							if (null != bi)
								depMap[bi.path] = bi;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.Message);
			}
		}

		public void Export(string file)
		{
			StreamWriter sw = File.CreateText(file);
			foreach (var kvp in depMap)
			{
				BundleInfo info = kvp.Value;
				sw.WriteLine(info.ToString());
			}

			sw.Close();
		}

		public bool ContainsBundle(string bundlePath)
		{
			return depMap.ContainsKey(bundlePath);
		}

		public void AddBundle(BundleInfo bundleInfo)
		{
			if (!depMap.ContainsKey(bundleInfo.path))
				depMap.Add(bundleInfo.path, bundleInfo);
		}

		#endregion

		public static string GetBundlePath(string path)
		{
#if UNITY_EDITOR
			if (null != TrdPath)
			{
				string bundlePath = FileUtils.GetFileReadPath(TrdPath + path, false);
				if (File.Exists(bundlePath))
					return bundlePath;
			}
#endif
			return FileUtils.GetFileReadFullPath(string.Format("{0}{1}", RootPath, path));
		}

		public BundleInfo GetInfo(string bundlePath, bool isManifest)
		{
			if (depMap.ContainsKey(bundlePath))
				return depMap[bundlePath];
			else if (isManifest)
			{
				BundleInfo bi = new BundleInfo();
				bi.depends = new string[] { };
				bi.mainName = "AssetBundleManifest";
				bi.type = AssetType.MANIFEST;
				depMap[bundlePath] = bi;
				return bi;
			}
			else
			{
				Log.Warning($"{bundlePath} not find");
#if UNITY_EDITOR
				Debug.LogError(string.Format("{0} not find", bundlePath));
#endif
				return null;
			}
		}
	}
}

