using System;
using Plugins.XAsset;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ImageWrapper : BaseWrapper<Image>,IFieldChangeCb<string>
    {

        public ImageWrapper(Image image) : base(image)
        {
            _view = image;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => _view.sprite = Assets.Load<Sprite>(value);
        }
    }
}
