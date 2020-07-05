using System;
using System.Collections.Generic;
using UnityEngine;
using UObj = UnityEngine.Object;

namespace Framework
{
    public class CommonResMgr : MonoSingleton<CommonResMgr>
    {
        #region Var

        public Font[] uiFonts;

        public string[] shaderAssets;
        public string fontAsset; //字体，目前一个也ok
        private bool isFontLoaded;

        private string[] atlasAssets;

        private Dictionary<string, Sprite> cache = new Dictionary<string, Sprite>();
        private Dictionary<string, Font> fonts = new Dictionary<string, Font>();

        public Font mainFont { get; private set; }

        #endregion

        #region Public Method

        protected void Awake()
        {
            for (int i = 0; i < uiFonts.Length; ++i)
            {
                Font f = uiFonts[i];
                if (null != f)
                {
                    if (!fonts.ContainsKey(f.name))
                        fonts.Add(f.name, f);
                }
            }
        }

        public void CacheAtlas(string[] atlasRess)
        {
            atlasAssets = atlasRess;
        }

        public void Cache(Action callback)
        {
            int count = atlasAssets?.Length ?? 0;
            count += shaderAssets?.Length ?? 0;
            if (!string.IsNullOrEmpty(fontAsset) && !isFontLoaded)
                count++;
            if (count == 0)
            {
                if (null != callback) callback();
                return;
            }

            Action done = () =>
            {
                --count;
                if (count == 0 && null != callback) callback();
            };
            if (null != atlasAssets)
            {
                for (int i = 0; i < atlasAssets.Length; ++i)
                {
                    LoadAtlas(atlasAssets[i], done);
                }
            }

            if (null != shaderAssets)
            {
                for (int i = 0; i < shaderAssets.Length; ++i)
                {
                    LoadShader(shaderAssets[i], done);
                }
            }

            if (!string.IsNullOrEmpty(fontAsset))
                LoadFont(done);
        }

        public Sprite GetSprite(string name)
        {
            if (null == name)
                return null;
            if (cache.ContainsKey(name))
                return cache[name];
            return null;
        }

        public Font GetFont(string name)
        {
            if (fonts.ContainsKey(name))
                return fonts[name];
            return null;
        }

        #endregion

        #region Private Method

        private void LoadAtlas(string atlasRes, Action cb = null)
        {
            string assetPath = ResObjUtil.GetObjPath(atlasRes);
            BundleMgr.Instance.GetAsset(assetPath, (objs, cbId) =>
            {
                UObj[] assets = (UObj[]) objs;
                foreach (var asset in assets)
                {
                    Sprite sprite = asset as Sprite;
                    if (null != sprite)
                    {
                        if (cache.ContainsKey(sprite.name))
                            this.Msg($"atlas {atlasRes} sprite {sprite.name} already added");
                        else
                            cache.Add(sprite.name, sprite);
                    }
                }

                if (null != cb)
                    cb();
            });
        }

        private void CacheSprite(List<Sprite> sprites)
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                cache.Add(sprites[i].name, sprites[i]);
            }
        }

        private void RemoveSprite(List<Sprite> sprites)
        {
            for (int i = 0; i < sprites.Count; ++i)
            {
                cache.Remove(sprites[i].name);
            }
        }

        private void LoadShader(string shaderAsset, Action cb = null)
        {
            string assetPath = ResObjUtil.GetObjPath(shaderAsset);
            if (null != assetPath)
            {
                BundleMgr.Instance.GetAsset(assetPath, (assets, cbId) =>
                {
                    //暂时注释掉，现在会影响加载速度，预计替换为ShaderVariantCollection
                    //Shader.WarmupAllShaders();
                    if (null != cb)
                        cb();
                });
            }
            else
            {
                cb?.Invoke();
            }
        }

        public void LoadFont(Action cb = null)
        {
            string assetPath = ResObjUtil.GetObjPath(fontAsset);
            BundleMgr.Instance.GetAsset(assetPath, (asset, cbId) =>
            {
                isFontLoaded = true;
                Font f = asset as Font;
                mainFont = f;
                if (null != f)
                {
                    if (!fonts.ContainsKey(f.name))
                        fonts.Add(f.name, f);
                }

                if (null != cb)
                    cb();
            });
        }

        #endregion
    }
}
