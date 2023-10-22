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
                if (string.IsNullOrEmpty(path)) return;
                (Container as View).ResComponent.LoadAsset<Sprite>(path).Callbackable()
                    .OnCallback(result =>
                    {
                        if (result.IsCancelled) return;
                        if (Component != null)
                        {
                            Component.sprite = result.Result;
                        }
                    });
            };
        }
    }
}