using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UObj = UnityEngine.Object;

namespace Framework
{
    public class SepResWrapper : MonoBehaviour
    {
        public enum ResType : int
        {
            NONE = 0,
            FONT = 1,
            IMG = 2,
        }
        [Serializable]
        public struct SepRes
        {
            public UObj wrapper;
            public ResType resType;
            public string resName;
            public void Check()
            {
                switch (resType)
                {
                    case ResType.FONT:
                        this.CheckTextFont();
                        break;
                    case ResType.IMG:
                        this.CheckImgSprite();
                        break;
                }
            }
            private void CheckTextFont()
            {
                Text text = wrapper as Text;
                if (null == text)
                    return;
                if (!string.IsNullOrEmpty(resName))
                    text.font = CommonResMgr.Ins.GetFont(resName);
            }
            private void CheckImgSprite()
            {
                Image img = wrapper as Image;
                if (null == img)
                    return;
                if (!string.IsNullOrEmpty(resName))
                    img.sprite = CommonResMgr.Ins.GetSprite(resName);
            }
        }
        public List<SepRes> sepRess;
        public static void Attach(GameObject go)
        {
            SepResWrapper wrapper = go.GetComponent<SepResWrapper>();
            if (null == wrapper)
                wrapper = go.AddComponent<SepResWrapper>();
            wrapper.sepRess = new List<SepRes>();
            wrapper.CheckAllText();
            wrapper.CheckAllImg();
        }
        private void CheckAllText()
        {
            var texts = gameObject.GetComponentsInChildren<Text>().ToList();
            foreach (Text text in texts)
            {
                if (text.font != null&&text.font.name != "Arial")
                {
                    SepRes sepRes = new SepRes()
                    {
                        wrapper = text,
                        resType = ResType.FONT,
                        resName = text.font.name
                    };
                    sepRess.Add(sepRes);
                    text.font = null;
                }
            }
        }
        private void CheckAllImg()
        {
            var imgs = gameObject.GetComponentsInChildren<Image>().ToList();
            foreach (Image img in imgs)
            {
                if (img.sprite != null)
                {
                    SepRes sepRes = new SepRes()
                    {
                        wrapper = img,
                        resType = ResType.IMG,
                        resName = img.sprite.name
                    };
                    sepRess.Add(sepRes);
                    img.sprite = null;
                }
            }
        }
        void Awake()
        {
            if(null!=sepRess)
            {
                for (int i = 0; i < sepRess.Count; ++i)
                    sepRess[i].Check();
            }
        }
    }
}
