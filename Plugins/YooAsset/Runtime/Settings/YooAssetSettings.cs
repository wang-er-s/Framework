﻿using UnityEngine;

namespace YooAsset
{
	[CreateAssetMenu(fileName = "YooAssetSettings", menuName = "YooAsset/Create Settings")]
	public class YooAssetSettings : ScriptableObject
	{
		/// <summary>
		/// AssetBundle文件的后缀名
		/// </summary>
		public string AssetBundleFileVariant = "bundle";

		/// <summary>
		/// 原生文件的后缀名
		/// </summary>
		public string RawFileVariant = "rawfile";

		/// <summary>
		/// 构建输出的补丁清单文件名称
		/// </summary>
		public string PatchManifestFileName = "PatchManifest";

		/// <summary>
		/// 构建输出的Unity清单文件名称
		/// </summary>
		public string UnityManifestFileName = "UnityManifest";


		/// <summary>
		/// 构建输出的报告文件
		/// </summary>
		public const string ReportFileName = "BuildReport";

		/// <summary>
		/// 静态版本文件
		/// </summary>
		public const string VersionFileName = "StaticVersion.bytes";

		/// <summary>
		/// Unity内置着色器资源包名称
		/// </summary>
		public const string UnityBuiltInShadersBundleName = "UnityBuiltInShaders.bundle";

		/// <summary>
		/// PlayMode
		/// </summary>
		/// <returns></returns>
		public YooAssets.EPlayMode PlayMode = YooAssets.EPlayMode.EditorSimulateMode;
	}
}