using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Nine.ResKit
{
    public delegate void OnAssetGot(System.Object asset, ulong cbId);
    public class ResMgr
    {
        public ulong GetAsset(string assetPath, OnAssetGot cb, bool isManifest = false)
        {
            
            return 0;
        }

        public object GetAssetSync(string assetPath)
        {

            return null;
        }

        public void RecycleAsset(string assetPath)
        {
            
        }

        public void DeleteNoRefBundles()
        {
            
        }

        public void Gc(bool delNoRef)
        {
            if (delNoRef)
                DeleteNoRefBundles();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}
