﻿using UnityEngine;

namespace YooAsset
{
	public static class YooAssetSettingsData
	{
		private static YooAssetSettings _setting = null;
		public static YooAssetSettings Setting
		{
			get
			{
				if (_setting == null)
					LoadSettingData();
				return _setting;
			}
		}

		/// <summary>
		/// 加载配置文件
		/// </summary>
		private static void LoadSettingData()
		{
			_setting = Resources.Load<YooAssetSettings>("YooAssetSettings");
			if (_setting == null)
			{
				YooLogger.Log("YooAsset use default settings.");
				_setting = ScriptableObject.CreateInstance<YooAssetSettings>();
			}
			else
			{
				YooLogger.Log("YooAsset use user settings.");
			}
		}

		/// <summary>
		/// 获取构建报告文件名
		/// </summary>
		public static string GetReportFileName(string packageName, string packageVersion)
		{
			return $"{YooAssetSettings.ReportFileName}_{packageName}_{packageVersion}.json";
		}

		/// <summary>
		/// 获取清单文件完整名称
		/// </summary>
		public static string GetManifestBinaryFileName(string packageName, string packageVersion)
		{
			return $"{Setting.PatchManifestFileName}_{packageName}_{packageVersion}.bytes";
		}

		/// <summary>
		/// 获取清单文件完整名称
		/// </summary>
		public static string GetManifestJsonFileName(string packageName, string packageVersion)
		{
			return $"{Setting.PatchManifestFileName}_{packageName}_{packageVersion}.json";
		}

		/// <summary>
		/// 获取包裹的哈希文件完整名称
		/// </summary>
		public static string GetPackageHashFileName(string packageName, string packageVersion)
		{
			return $"{Setting.PatchManifestFileName}_{packageName}_{packageVersion}.hash";
		}

		/// <summary>
		/// 获取包裹的版本文件完整名称
		/// </summary>
		public static string GetPackageVersionFileName(string packageName)
		{
			return $"{Setting.PatchManifestFileName}_{packageName}.version";
		}

		public static void Save()
		{
#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(Setting);
			UnityEditor.AssetDatabase.SaveAssets(); 
#endif
		}
	}
}