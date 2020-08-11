using System;
using System.IO;
using Framework.Net;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.CommonHelper
{
    public class UIHelper
    {
        public static void TextTyperEffect(Text textCommponent, string content, float delta, Action onComplete)
        {
            if (null != textCommponent && !string.IsNullOrEmpty(content))
            {
                textCommponent.text = "";
                int length = 1;
                Timer timer = null;
                timer = Timer.RunBySeconds(delta, () =>
                {
                    var subContent = content.Substring(0, length);
                    textCommponent.text = subContent;
                    length++;
                    if (length > content.Length)
                    {
                        timer.Stop();
                        onComplete?.Invoke();
                    }
                }, null);
            }
        }
        
        /// <summary>
        /// 切割一张Texure大图上面的一部分，作为Sprite精灵返回
        /// </summary>
        /// <returns></returns>
        public static Sprite SliceTextureToSprite(Texture2D texture2D, float x, float y, float width, float height)
        {
            if (null != texture2D)
            {
                if (x + width > texture2D.width)
                {
                    width = texture2D.width - x;
                    Log.Warning("the width is larger then texture2D width!");
                }
                if (y + height > texture2D.height)
                {
                    height = texture2D.height - y;
                    Log.Warning("the height is larger then texture2D height!");
                }

                Sprite sprite = Sprite.Create(texture2D, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));
                return sprite;
            }
            Log.Warning("Texture2D 不能为空！");
            
            return null;
        }
    }
}