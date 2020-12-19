using System.IO;
using UnityEditor;
using UnityEngine;
using UnityIO;

namespace Framework.Editor
{
    [InitializeOnLoad]
    public class GenConfigAsset
    {
        static GenConfigAsset()
        {
            ConfigBase.Load<BuildConfig>();
            ConfigBase.Load<UIConfig>();
        }
    }
}