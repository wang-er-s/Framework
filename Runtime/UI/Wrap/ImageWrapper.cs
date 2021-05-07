using System;
using Framework.Assets;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
{
    public class ImageWrapper : BaseWrapper<Image>, IFieldChangeCb<string>
    {

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return path =>
            {
                IRes res = null;
                if (Container is ICustomRes customRes)
                {
                    res = customRes.GetRes();
                }
                else
                {
                    res = Res.Default;
                }
                res.LoadAssetAsync<Sprite>(path).Callbackable()
                    .OnCallback(result => Component.sprite = result.Result);
            };
        }

        public ImageWrapper(Image component, View view) : base(component, view)
        {
        }
    }
}