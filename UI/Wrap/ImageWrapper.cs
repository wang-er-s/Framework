using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugins.XAsset;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ImageWrapper : BaseWrapper<Image>,IFieldChangeCb<string>
    {

        private readonly Image image;

        public ImageWrapper(Image _image) : base(_image)
        {
            image = _image;
        }

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return (value) => image.sprite = Assets.Load<Sprite>(value);
        }
    }
}
