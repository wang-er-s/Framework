using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
	[ShowOdinSerializedPropertiesInInspector]
	public class FrameworkRuntimeConfig : ConfigBase
	{
		[LabelText("资源加载类型")]
		public ResType LoadType = ResType.Resources;

		[FoldoutGroup("热更设置", expanded: true)]
		[HideLabel]
		[InlinePropertyAttribute]
		public ILRConfig ILRConfig = new ILRConfig();

		[LabelText("主项目dll名字")]
		public string GameDllName = "GamePlay";

		public enum ResType
		{
			Resources,
#if ADDRESSABLE
			Addressable,
#endif
#if XASSET
			XAsset,
#endif
#if UNITY_EDITOR
			Editor,
#endif
		}
	}

	[Serializable]
	public class ILRConfig
	{
		[LabelText("是否使用热更dll")]
		public bool UseHotFix = false;

		[LabelText("是否使用pbd")]
		public bool UsePbd = true;

		[LabelText("热更dll名字")]
		public string DllName = "Game@hotfix";

		[LabelText("是否自动编译热更dll")]
		public bool AutoCompile;

		[FolderPath(AbsolutePath = false)]
		[LabelText("生成适配器地址")]
		public string AdaptorPath;

		[ReadOnly]
		public bool ReleaseBuild = false;
	}
}