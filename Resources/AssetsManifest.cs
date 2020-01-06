using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AD
{
    [Serializable]
    public class AssetData
    { 
        public int bundle;
        public int dir;
        public string name;
    }
    
    [CreateAssetMenu(fileName = "Manifest", menuName = "ScriptableObjects/Manifest", order = 1)]
    public class AssetsManifest : ScriptableObject
    {
        public string downloadURL = "";
        [ReadOnly]
        public string[] bundles = new string[0];
        [ReadOnly]
        public string[] dirs = new string[0];
        [ReadOnly]
        public AssetData[] assets = new AssetData[0];
    }
}
