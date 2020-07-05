using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    public enum BundleType
    {
        Single,//资源打包成一个，使用目录名
        Multi,//每个资源独立打包
        MultiMulti,//每个资源独立打包，但每个资源本身是由多object构成
    }

    public enum BundleRef
    {
        Ignore,//不管是否有引用都打包
        MultiRef,//只有有多份引用的时候才打包
        HasRef,//只要有引用，就打包
    }
    
    public class ResDataHelp
    {
        //".png|.tga|.anim|.fbx|.asset|.psd|.jpg|.tif|.bmp|.mat";
        private static readonly string[] texture = { ".png",".tga",".jpg",".bmp",".psd",".tif",".exr"};
        private static readonly string[] sound = {".wav",".mp3"};
        private static readonly string[] font = {".ttf",".fontsettings"};
        private static readonly string[] gameobj = {".prefab",".fbx"};
        private static readonly string anim = ".anim";
        private static readonly string[] shader = {".shader",".shadergraph"};
        private static readonly string[] shader_parts = {".shader",".shadergraph",".compute",".shadervariants"};
        private static readonly string material = ".mat";
        private static readonly string sprite_atlas = ".spriteatlas";
        private static readonly string customAsset = ".asset";
        private static readonly string[] obj = {".fbx"};
        
        public static AssetType GetAssetType(string ext)
        {
            if (Array.IndexOf(texture, ext) >= 0)
                return AssetType.TEXTURE;
            if (Array.IndexOf(sound, ext) >= 0)
                return AssetType.SOUND;
            if (ext == anim)
                return AssetType.ANIM;
            if (Array.IndexOf(font, ext) >= 0)
                return AssetType.FONT;
            if (Array.IndexOf(gameobj, ext) >= 0)
                return AssetType.GAMEOBJ;
            if (Array.IndexOf(shader,ext)>=0)
                return AssetType.SHADER;
            if (ext == material)
                return AssetType.MATERIAL;
            if (ext == sprite_atlas)
                return AssetType.SPRITE_ATLAS;

            return AssetType.OBJECT;
        }

        public static bool IsCustomAsset(string ext)
        {
            return ext == customAsset;
        }
        
        public static string GetBundleRedirectName(string bundleName)
        {
            bundleName = bundleName.ToLower();
            bundleName = ResNameRedirect.GetRedirectName(bundleName);

            return bundleName+BundleConfig.bundleFileExt;
        }
    }

    #region Dirs
    [Serializable]
    public abstract class OneDir
    {
        public string path = "";
        public bool ignore;
    }
    [Serializable]
    public class ResDir : OneDir
    {
        public string filePattern = "*.*";
        public BundleRef bundleRef;
        public BundleType bundleType;
    }
    [Serializable]
    public class PrefabDir : OneDir
    {}
    [Serializable]
    public class SceneDir : OneDir
    {}
    #endregion

    #region Info

    public enum InfoType : int
    {
        RES = 0,
        PREFAB = 1,
        SCENE = 2,
    }
    public abstract class OneInfo
    {
        public abstract InfoType Type { get; }
        public string res;
        public List<OneInfo> depends;
        public int refCount;
        public List<string> refBy;
        public bool bundleExclude = false;//打包排除
        #region Temp Var
        private string body;
        private string fileName;
        protected string bundleName;

        public string BundleName
        {
            get
            {
                if (null != bundleName) return bundleName;
                bundleName = NeedBundle() ? ResDataHelp.GetBundleRedirectName(GetBundleName()) : string.Empty;
                return bundleName;
            }
        }

        #endregion
        protected string GetBody()
        {
            if (null == body)
            {
                int index = res.LastIndexOf('.');
                body = res.Substring(0,index);
            }

            return body;
        }

        protected string GetFileName()
        {
            if (null == fileName)
            {
                string[] parts = GetBody().Split(new[] {'/'});
                fileName = parts[parts.Length - 1];
            }

            return fileName;
        }

        protected virtual string GetAssetName()
        {
            return GetFileName();
        }
        
        public void AddDepend(OneInfo info)
        {
            if(null == depends)
                depends = new List<OneInfo>();
            if(!depends.Contains(info))
                depends.Add(info);
        }

        public void RemoveDepend(OneInfo info)
        {
            if(null == depends)
                return;
            if (depends.Contains(info))
                depends.Remove(info);
        }
        public virtual bool NeedBundle()
        {
            return true;
        }
        
        protected virtual string GetBundleName()
        {
            return res;
        }

        public abstract AssetType GetAssetType();

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(res);
        }
        
        public void GenConfig(BundleConfig config)
        {
            if (string.IsNullOrEmpty(BundleName))
                return;
            if(config.ContainsBundle(BundleName))
                return;
            BundleInfo info = new BundleInfo();
            info.path = BundleName;
            info.mainName = GetAssetName();
            info.type = GetAssetType();
            if (null != depends)
            {
                info.depends = depends
                    .Where(dep => !string.IsNullOrEmpty(dep.BundleName))
                    .Select(dep => dep.BundleName).ToArray();
            }
            config.AddBundle(info);
        }
        public void SetAssetBundleName(BundleBuild build)
        {
            if(string.IsNullOrEmpty(BundleName))
                return;
            build.SetBundle(res,BundleName);
        }
        public void RefBy(string who)
        {
            if(null == refBy)
                refBy = new List<string>();
            if(refBy.Contains(who))
                return;
            ++refCount;
            refBy.Add(who);
        }

        public void UnRefBy(string who)
        {
            if(null == refBy)
                return;
            if (refBy.Contains(who))
            {
                --refCount;
                refBy.Remove(who);
            }
            
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}--{1}--bundle:{2}:{3}", res, refCount,BundleName,bundleExclude);
            if (null != refBy)
            {
                sb.Append("\n\tRefBy:");
                for (int i = 0; i < refBy.Count; ++i)
                {
                    sb.AppendFormat("\n\t\t{0}",refBy[i]);
                }
            }

            if (null != depends)
            {
                sb.Append("\n\tDepends:");
                for (int i = 0; i < depends.Count; ++i)
                {
                    sb.AppendFormat("\n\t\t{0}",depends[i].res);
                }
            }

            sb.Append("\n");
            return sb.ToString();
        }
    }
    public class ResInfo : OneInfo
    {
        public override InfoType Type => InfoType.RES;
        public ResDir dir;
        
        #region Temp Var
        private string ext;
        private AssetType resType = AssetType.UNKNOW;
        #endregion

        private string GetExt()
        {
            if (null == ext)
            {
                int index = res.LastIndexOf('.');
                ext = res.Substring(index).ToLower();
            }

            return ext;
        }

        public override bool NeedBundle()
        {
            if (null == dir)
            {
                return refCount > 1;
            }
            else
            {
                switch (dir.bundleRef)
                {
                    case BundleRef.HasRef:
                        if (refCount == 0)
                            return false;
                        break;
                    case BundleRef.MultiRef:
                        if (refCount <= 1)
                            return false;
                        break;
                }
            }
            return true;
        }
        [Obsolete("def not clear")]
        public bool MayHasDepend()
        {
            if (IsCustomAsset())
                return true;
            AssetType assetType = GetAssetType();
            //对于AssetType.Object为未知的情况
            if (assetType == AssetType.MATERIAL || assetType == AssetType.OBJECT)
                return true;
            return false;
        }

        public bool IsSpriteAtlas()
        {
            return GetAssetType() == AssetType.SPRITE_ATLAS;
        }

        public bool IsTexture()
        {
            return GetAssetType() == AssetType.TEXTURE;
        }

        private bool IsCustomAsset()
        {
            return ResDataHelp.IsCustomAsset(GetExt());
        }

        public bool IsMaterial()
        {
            AssetType assetType = GetAssetType();
            return assetType == AssetType.MATERIAL;
        }

        protected override string GetBundleName()
        {
            if (null == dir)
                return base.GetBundleName();
            switch (dir.bundleType)
            {
                case BundleType.Single:
                    return dir.path;
                default:
                {
                    return base.GetBundleName();
                }
            }
        }

        public override AssetType GetAssetType()
        {
            if (resType == AssetType.UNKNOW)
            {
                if (IsCustomAsset())
                {
                    // TODO wangliang on 2019/07/14 17:07:11: 不能简单通过AssetDatabase.LoadAllAssetsAtPath获得数量来判断,
                    // 因为有可能打包的时候加入更多
                    // 可以采取的做法是 1：约定，2。...
                    resType = AssetType.MULTI_ASSETS;
                }
                else if (null == dir || dir.bundleType == BundleType.Multi)
                {
                    resType = ResDataHelp.GetAssetType(GetExt());
                }
                else
                    resType = AssetType.MULTI_ASSETS;
            }

            return resType;
        }

        protected override string GetAssetName()
        {
            if (null != dir && dir.bundleType == BundleType.Single)
                return BundleName;
            return base.GetAssetName();
        }
    }

    public class PrefabInfo : OneInfo
    {
        public override InfoType Type => InfoType.PREFAB;
        public PrefabDir dir;
        public override AssetType GetAssetType()
        {
            return AssetType.GAMEOBJ;
        }

        public override bool NeedBundle()
        {
            if (null == dir)
            {
                return refCount > 1;
            }
            return true;
        }
    }
    
    public class SceneInfo : OneInfo
    {
        public override InfoType Type => InfoType.SCENE;
        public SceneDir dir;

        public override AssetType GetAssetType()
        {
            return AssetType.SCENE;
        }
    }
    #endregion
}