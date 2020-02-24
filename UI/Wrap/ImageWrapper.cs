using System;
using Plugins.XAsset;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ImageWrapper : BaseWrapper<Image>,IFieldChangeCb<string>
    {
        public static Func<string, Sprite> LoadSpriteFunc;
        public ImageWrapper(Image image) : base(image)
        {
            _view = image;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (path) =>
                _view.sprite = LoadSpriteFunc == null ? Resources.Load<Sprite>(path) : LoadSpriteFunc(path);
        }
    }
}
