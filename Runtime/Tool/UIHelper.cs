using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    public class UIHelper
    {
        public static void TextTyperEffect(Text textComponent, string content, long delta, Action onComplete)
        {
            if (null != textComponent && !string.IsNullOrEmpty(content))
            {
                textComponent.text = "";
                int length = 1;
                ITimerExecutor timerExecutor = new CoroutineTimerExecutor();
                IAsyncResult asyncResult = null;
                asyncResult = timerExecutor.FixedRateAtDuration(deltaTime =>
                {
                    var subContent = content.Substring(0, length);
                    textComponent.text = subContent;
                    length++;
                    if (length > content.Length)
                    {
                        onComplete?.Invoke();
                        asyncResult.Cancel();
                    }
                }, period: delta);
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

        public static bool TouchInUI()
        {
            #if IPHONE || ANDROID
            			if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return true;
            #else
                    if (EventSystem.current.IsPointerOverGameObject()) return true;
            #endif
            return false;
        }
    }
}