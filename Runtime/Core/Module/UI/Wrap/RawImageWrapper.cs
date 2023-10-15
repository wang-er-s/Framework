using System;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class RawImageWrapper : BaseWrapper<RawImage>, IFieldChangeCb<string>
    {
        Action<string> IFieldChangeCb<string>.GetFieldChangeCb()
        {
            return path =>
            {
                if(string.IsNullOrEmpty(path)) return;
                (Container as View).ResComponent.LoadAssetAsync<Texture>(path).Callbackable()
                    .OnCallback(result =>
                    {
                        if(result.IsCancelled) return;
                        if (Component != null)
                        {
                            Component.texture = result.Result;
                        }
                    });
            };
        }
    }
}