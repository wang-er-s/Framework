using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UObj = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Framework
{
    public static class ResObjUtil
    {
        private static readonly Dictionary<string, string> cache = new Dictionary<string, string>();
        public static string GetObjPath(string res)
        {
            if (string.IsNullOrEmpty(res))
                return res;
#if UNITY_EDITOR
            if (!AppEnv.UseBundleInEditor)
            {
                if(File.Exists(BundleConfig.ProjectPath + res))
                    return res;
                return null;
            }
#endif
            if (cache.ContainsKey(res))
                return cache[res];
            else
            {
                string path = res;
                //int ext = path.LastIndexOf('.');
                //if (ext >= 0)
                //    path = path.Substring(0, ext);
                path = string.Format("{0}{1}", ResNameRedirect.GetRedirectName(path.ToLower()),BundleConfig.bundleFileExt);
                cache.Add(res, path);
                return path;
            }
        }

        public static bool IsResExist(string res)
        {
            return BundleMgr.Instance.HasBundle(GetObjPath(res));
        }
    }
    
    public abstract class AssetObj
    {
        protected string assetPath;
        protected ulong assetCbId;
        public string res { get; protected set; }
        public void Load(string res,Action onLoad)
        {
            this.res = res;
            assetPath = ResObjUtil.GetObjPath(res);
            if (null == assetPath)
            {
                if (null != onLoad)
                    onLoad();
            }
        #if UNITY_EDITOR
            if (!AppEnv.UseBundleInEditor)
            {
                LoadAssetAtEditor(onLoad);
                return;
            }
        #endif
            LoadAsync(onLoad);
        }

        public void Release()
        {
            if(assetCbId>0)
            {
                assetPath = null;
                BundleMgr.Instance.CancelUngotAsset(assetCbId);
                assetCbId = 0;
            }
            else
            {

#if UNITY_EDITOR
                if (!AppEnv.UseBundleInEditor)
                {
                    assetPath = null;
                    Clear();
                    return;
                }
#endif
                if (null!=assetPath)
                {
                    BundleMgr.Instance.ReleaseAsset(assetPath, false);
                    assetPath = null;
                    Clear();
                }
            }
        }

        private void LoadAsync(Action onLoad)
        {
            assetCbId = BundleMgr.Instance.GetAsset(assetPath, (obj, cbId) =>
            {
                assetCbId = 0;
                ProcessObj(obj);
                onLoad?.Invoke();
            });
        }
        protected abstract void LoadAssetAtEditor(Action onLoad);
        protected abstract void Clear();
        protected abstract void ProcessObj(object obj);
    }
    
    public class SingleAssetObj<T> : AssetObj where T: UnityEngine.Object
    {
        public T asset { get; private set; }

        protected override void ProcessObj(object obj)
        {
            asset = obj as T;
        }

        protected override void Clear()
        {
            asset = null;
        }

        protected override void LoadAssetAtEditor(Action onLoad)
        {
#if UNITY_EDITOR
            asset = AssetDatabase.LoadAssetAtPath<T>(res);
            onLoad?.Invoke();
#endif
        }
        public T LoadSync(string res)
        {
            this.res = res;
            assetCbId = 0;
            assetPath = ResObjUtil.GetObjPath(res);
            asset = BundleMgr.Instance.GetAssetSync(assetPath) as T;
            return asset;
        }
    }

    public abstract class MultiAssetObj<T> : AssetObj where T : UnityEngine.Object
    {
        protected abstract void ProcessEachAsset(T asset);
        protected override void ProcessObj(object obj)
        {
            UObj[] objs = (UObj[]) obj;
            foreach (var o in objs)
            {
                T tAsset = o as T;
                if (null != tAsset)
                {
                    ProcessEachAsset(tAsset);
                }
            }
        }

        protected override void LoadAssetAtEditor(Action onLoad)
        {
#if UNITY_EDITOR
            UObj[] objs = AssetDatabase.LoadAllAssetsAtPath(res);
            foreach (var o in objs)
            {
                T tAsset = o as T;
                if (null != tAsset)
                {
                    ProcessEachAsset(tAsset);
                }
            }

            if (null != onLoad)
                onLoad();
#endif
        }
        
        public void LoadSync(string res)
        {
            this.res = res;
            assetCbId = 0;
            assetPath = ResObjUtil.GetObjPath(res);
            UObj[] objs = (UObj[])BundleMgr.Instance.GetAssetSync(assetPath);
            foreach (var o in objs)
            {
                T tAsset = o as T;
                if (null != tAsset)
                {
                    ProcessEachAsset(tAsset);
                }
            }
        }
    }

    public class SoundAsset : SingleAssetObj<AudioClip>
    {
    }
    public class TextureAsset : SingleAssetObj<Texture>
    {
    }
    
    public class AnimationClipAsset : SingleAssetObj<AnimationClip>
    {
    }

    public class SceneAsset : SingleAssetObj<AssetBundle>
    {
        protected override void LoadAssetAtEditor(Action onLoad)
        {
            throw new Exception("can't load scene from ");
        }

        public IEnumerator DelayRelease()
        {
            return IEnumeratorUtil.DelayAction(() =>
            {
                asset.Unload(false);
            });
        }
    }

    public class UiSpriteSingleAsset : MultiAssetObj<Sprite>
    {
        public Sprite asset;
        protected override void Clear()
        {
            asset = null;
        }

        protected override void ProcessEachAsset(Sprite asset)
        {
            this.asset = asset;
        }
    }

    public class UiSpriteMultiAsset : MultiAssetObj<Sprite>
    {
        public Dictionary<string, Sprite> assets;

        protected override void Clear()
        {
            assets = null;
        }

        protected override void ProcessEachAsset(Sprite asset)
        {
            if(null == assets)
                assets = new Dictionary<string, Sprite>();
            if (assets.ContainsKey(asset.name))
            {
                Debug.LogWarning($"sprite {asset.name} already has");
            }
            else
            {
                assets.Add(asset.name,asset);
            }
        }

        public Sprite GetSprite(string spriteName)
        {
            if (string.IsNullOrEmpty(spriteName))
                return null;
            return assets.ContainsKey(spriteName) ? assets[spriteName] : null;
        }
    }

    public class ShaderAsset : MultiAssetObj<Shader>
    {
        protected override void Clear()
        {
        }

        protected override void ProcessEachAsset(Shader asset)
        {
        }
    }
}
