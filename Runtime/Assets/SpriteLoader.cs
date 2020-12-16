using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Asynchronous;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework.Assets
{
    public class SpriteLoader
    {
        private readonly List<AsyncOperationHandle> _spriteAssets = new List<AsyncOperationHandle>();
        private AddressableRes addressableRes;

        private async void LoadSpriteAsync(string path, IProgressPromise<float, Sprite> promise)
        {
            var tuple = ParsePath(path);
            string _path = tuple.Item1;
            string spriteName = tuple.Item2;
            Sprite sprite = null;
            if (string.IsNullOrEmpty(_path))
            {
                var operation = Addressables.LoadAssetAsync<Sprite>(_path);
                while (!operation.IsDone)
                {
                    promise.UpdateProgress(operation.PercentComplete);
                    await Task.Yield();
                }
                _spriteAssets.Add(operation);
                sprite = operation.Result;
            }
            else
            {
                var operation = Addressables.LoadAssetAsync<IList<Sprite>>(_path);
                while (!operation.IsDone)
                {
                    promise.UpdateProgress(operation.PercentComplete);
                    await Task.Yield();
                }
                _spriteAssets.Add(operation);
                foreach (var sp in operation.Result)
                {
                    if (sp.name != spriteName) continue;
                    sprite = sp;
                    break;
                }
            }
            promise.SetResult(sprite);
        }

        public IProgressResult<float,Sprite> LoadSpriteAsync(string path)
        {
            ProgressResult<float, Sprite> promise = new ProgressResult<float, Sprite>();
            LoadSpriteAsync(path, promise);
            return promise;
        }

        [Obsolete("仅做展示，暂时不实用同步加载",true)]
        public Sprite LoadSprite(string path)
        {
            var tuple = ParsePath(path);
            string _path = tuple.Item1;
            string spriteName = tuple.Item2;
            return null;
        }

        public void Release()
        {
            for (int i = 0; i < _spriteAssets.Count; i++)
            {
                try
                {
                    Addressables.Release(_spriteAssets[i]);
                    _spriteAssets.RemoveAt(i);
                    i--;
                }
                catch (Exception)
                {
                }
            }
            _spriteAssets.Clear();
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
        
    }
}