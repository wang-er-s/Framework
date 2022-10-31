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
                IRes res = null;
                if (Container is ICustomRes customRes)
                {
                    res = customRes.GetRes();
                }
                else
                {
                    throw new Exception($"{Container} need extend ICustomRes");
                }
                res.LoadAssetAsync<Texture>(path).Callbackable()
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

        public RawImageWrapper(RawImage component, View view) : base(component, view)
        {
        }
    }
}