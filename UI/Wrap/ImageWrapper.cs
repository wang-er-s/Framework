using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace AD.UI.Wrap
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
            //TODO 图片资源加载方式
            return (value) => image.sprite = string.IsNullOrEmpty(value) ? null : Resources.Load<Sprite>(value);
        }
    }
}
