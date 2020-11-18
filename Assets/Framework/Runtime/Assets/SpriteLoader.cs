using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public class SpriteLoader
    {
        private readonly Dictionary<string, SpriteAsset> _spriteAssets = new Dictionary<string, SpriteAsset>();

        public async Task<Sprite> LoadSprite(string path)
        {
            if (TryGetAsset(path, out var asset))
            {
                return asset.Sprite;
            }

            Sprite sprite = null;
            if (string.IsNullOrEmpty(asset.SpriteName))
            {
                var operation = Addressables.LoadAssetAsync<Sprite>(asset.Path);
                await operation;
                asset.Handle = operation;
                sprite = operation.Result;
            }
            else
            {
                var operation = Addressables.LoadAssetAsync<IList<Sprite>>(asset.Path);
                await operation;
                asset.Handle = operation;
                foreach (var sp in operation.Result)
                {
                    if (sp.name == asset.SpriteName)
                    {
                        sprite = sp;
                        break;
                    }
                }
            }

            asset.Sprite = sprite;
            _spriteAssets[path] = asset;
            return sprite;
        }

        public void LoadSprite(string path, Action<Sprite> callback)
        {
            if (TryGetAsset(path, out var asset))
            {
                callback(asset.Sprite);
            }

            if (string.IsNullOrEmpty(asset.SpriteName))
            {
                Addressables.LoadAssetAsync<Sprite>(asset.Path).Completed += operation =>
                {
                    asset.Handle = operation;
                    callback(operation.Result);
                    asset.Sprite = operation.Result;
                    _spriteAssets[path] = asset;
                };
            }
            else
            {
                Addressables.LoadAssetAsync<IList<Sprite>>(asset.Path).Completed += operation =>
                {
                    asset.Handle = operation;
                    foreach (var sp in operation.Result)
                    {
                        if (sp.name == asset.SpriteName)
                        {
                            callback(sp);
                            asset.Sprite = sp;
                            break;
                        }
                    }

                    _spriteAssets[path] = asset;
                };
            }
        }

        public void Release()
        {
            foreach (var spriteAsset in _spriteAssets.Values)
            {
                Addressables.Release(spriteAsset.Handle);
            }
        }

        private bool TryGetAsset(string path, out SpriteAsset asset)
        {
            if (_spriteAssets.TryGetValue(path, out asset))
            {
                return true;
            }

            asset = new SpriteAsset();
            var tuplePath = ParsePath(path);
            asset.Path = tuplePath.Item1;
            asset.SpriteName = tuplePath.Item2;
            return false;
        }

        private Tuple<string, string> ParsePath(string path)
        {
            var index = path.LastIndexOf("/", StringComparison.Ordinal);
            string _path = String.Empty;
            string spriteName = String.Empty;
            if (index != -1)
            {
                _path = path.Substring(0, index);
                spriteName = path.Substring(index + 1);
            }
            else
            {
                _path = path;
            }

            return new Tuple<string, string>(_path, spriteName);
        }

        private class SpriteAsset
        {
            public string Path;
            public string SpriteName;
            public AsyncOperationHandle Handle;
            public Sprite Sprite;
        }
    }
}