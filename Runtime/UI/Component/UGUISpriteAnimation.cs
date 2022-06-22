using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIComponent
{
    [RequireComponent(typeof(Image))]
    public class UGUISpriteAnimation : MonoBehaviour
    {
        private Image imageSource;
        private int mCurFrame = 0;
        private float mDelta = 0;
        public float FPS = 5;
        public List<Sprite> SpriteFrames;
        public bool IsPlaying { get; private set; }
        public bool Forward = true;
        public bool AutoPlay = true;
        public bool Loop = true;
        public float Interval = 0;
        public int FrameCount
        {
            get { return SpriteFrames.Count; }
        }

        void Awake()
        {
            imageSource = GetComponent<Image>();
        }

        void Start()
        {
            if (AutoPlay)
            {
                Play();
            }
            else
            {
                IsPlaying = false;
            }
        }

        private void SetSprite(int idx)
        {
            imageSource.sprite = SpriteFrames[idx];
        }

        public void Play()
        {
            IsPlaying = true;
            Forward = true;
        }

        public void PlayReverse()
        {
            IsPlaying = true;
            Forward = false;
        }

        void Update()
        {
            if (!IsPlaying || 0 == FrameCount)
            {
                return;
            }
            mDelta += Time.deltaTime;
            if (mCurFrame == FrameCount - 1 && mDelta < Interval)
            {
                return;
            }
            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                if (Forward)
                {
                    mCurFrame++;
                }
                else
                {
                    mCurFrame--;
                }
                if (mCurFrame >= FrameCount)
                {
                    if (Loop)
                    {
                        mCurFrame = 0;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }
                else if (mCurFrame < 0)
                {
                    if (Loop)
                    {
                        mCurFrame = FrameCount - 1;
                    }
                    else
                    {
                        IsPlaying = false;
                        return;
                    }
                }
                SetSprite(mCurFrame);
            }
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Resume()
        {
            if (!IsPlaying)
            {
                IsPlaying = true;
            }
        }

        public void Stop()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            IsPlaying = false;
        }

        public void Rewind()
        {
            mCurFrame = 0;
            SetSprite(mCurFrame);
            Play();
        }
    }
}