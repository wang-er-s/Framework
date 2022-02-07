using System;
using Framework.Assets;
using Framework.UI.Core;
using Framework.UI.Wrap.Base;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI.Wrap
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
                    res = Res.Default;
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