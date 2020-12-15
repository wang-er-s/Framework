using System;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ImageWrapper : BaseWrapper<Image>, IFieldChangeCb<string>
    {

        public ImageWrapper(Image image) : base(image)
        {
            View = image;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return path => View.sprite = Resources.Load<Sprite>(path);
        }
    }
}