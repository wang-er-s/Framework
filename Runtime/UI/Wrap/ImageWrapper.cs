using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class ImageWrapper : BaseWrapper<Image>, IFieldChangeCb<string>
    {

        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return path =>
            {
                if(string.IsNullOrEmpty(path)) return;
                IRes res = null;
                if (Container is ICustomRes customRes)
                {
                    res = customRes.GetRes();
                }
                else
                {
                    throw new Exception($"{Container} need extend ICustomRes");
                }
                res.LoadAssetAsync<Sprite>(path).Callbackable()
                    .OnCallback(result =>
                    {
                        if(result.IsCancelled) return;
                        if (Component != null)
                        {
                            Component.sprite = result.Result;
                        }
                    });
            };
        }

        public ImageWrapper(Image component, View view) : base(component, view)
        {
        }
    }
}