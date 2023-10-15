using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework
{
	[ShowOdinSerializedPropertiesInInspector]
	public class FrameworkRuntimeConfig : ConfigBase
	{
		[LabelText("主项目dll名字")]
		public string GameDllName = "GamePlay";

		[LabelText("游戏版本")]
		public string GameVersion = "v1.0";
	}
}