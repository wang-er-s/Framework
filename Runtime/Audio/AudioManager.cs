﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{
    public sealed class AudioManager : Singleton<AudioManager> , ISingletonAwake
    {

        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        private int BackgroundPriority = 0;

        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        private int SinglePriority = 10;

        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        private int MultiplePriority = 20;

        /// <summary>
        /// 世界音效优先级
        /// </summary>
        private int WorldPriority = 30;

        private float _backgroundVolume = 0.6f;
        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume
        {
            get => _backgroundVolume;
            set
            {
                _backgroundVolume = value;
                _backgroundAudio.Volume = value;
            }
        }
        /// <summary>
        /// 音效音量
        /// </summary>
        public float SoundEffectVolume = 1;

        private AudioSourcePlayer _backgroundAudio;
        private AudioSourcePlayer _singleAudio;
        private readonly List<AudioSourcePlayer> _multipleAudio = new List<AudioSourcePlayer>();
        private readonly Dictionary<GameObject, AudioSource> _worldAudio = new Dictionary<GameObject, AudioSource>();
        private bool _isMute;
        private GameObject gameObject;
        private IRes res;

        void ISingletonAwake.Awake()
        {
            gameObject = new GameObject("Audio");
            _backgroundAudio =
                new AudioSourcePlayer(CreateAudioSource("BackgroundAudio", BackgroundPriority, BackgroundVolume));
            _singleAudio = new AudioSourcePlayer(CreateAudioSource("SingleAudio", SinglePriority, SoundEffectVolume));
            res = Res.Create();
        }
        
        private class AudioSourcePlayer : IDisposable
        {
            private AudioSource _audioSource;

            public bool IsPlaying => _audioSource.isPlaying;

            public int Priority
            {
                get => _audioSource.priority;
                set => _audioSource.priority = value;
            }

            public float Volume
            {
                get => _audioSource.volume;
                set => _audioSource.volume = value;
            }

            public bool Mute
            {
                get => _audioSource.mute;
                set => _audioSource.mute = value;
            }

            private string clipPath;

            public AudioSourcePlayer(AudioSource audioSource)
            {
                this._audioSource = audioSource;
            }

            public bool IsSameClip(AudioClip clip)
            {
                return _audioSource.clip == clip;
            }

            public bool IsSameClip(string name)
            {
                return _audioSource.clip.name == name;
            }

            public bool IsSameClipPath(string clipPath)
            {
                return this.clipPath == clipPath;
            }

            public void SetParams(AudioClip clip, bool isLoop, float speed, string clipPath = "")
            {
                _audioSource.clip = clip;
                _audioSource.loop = isLoop;
                _audioSource.pitch = speed;
                _audioSource.spatialBlend = 0;
                this.clipPath = clipPath;
            }

            public void Play()
            {
                _audioSource.Play();
            }

            public void Stop()
            {
                _audioSource.Stop();
            }

            private void Release()
            {
                _audioSource.clip = null;
            }

            public void Dispose()
            {
                Release();
                Object.Destroy(_audioSource.gameObject);
                _audioSource = null;
            }
        }
        
        /// <summary>
        /// 静音
        /// </summary>
        public bool Mute
        {
            get => _isMute;
            set
            {
                _isMute = value;
                _backgroundAudio.Mute = value;
                _singleAudio.Mute = value;
                foreach (var audio in _multipleAudio)
                {
                    audio.Mute = value;
                }

                foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
                {
                    audio.Value.mute = value;
                }
            }
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public async void PlayBackgroundMusic(string clipName, bool isLoop = true, float speed = 1)
        {
            var clip = await res.LoadAsset<AudioClip>(clipName);
            if (_backgroundAudio.IsPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.SetParams(clip, isLoop, speed, clipName);
            _backgroundAudio.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            if (!_backgroundAudio.IsPlaying) return;
            // if (isGradual)
            // {
            //     _backgroundAudio.DOFade(0, 2);
            // }
            // else
            {
                _backgroundAudio.Volume = 0;
            }
        }

        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        public void UnPauseBackgroundMusic(bool isGradual = true)
        {
            if (!_backgroundAudio.IsPlaying) return;
            // if (isGradual)
            // {
            //     _backgroundAudio.DOFade(BackgroundVolume, 2);
            // }
            // else
            {
                _backgroundAudio.Volume = BackgroundVolume;
            }
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundAudio.IsPlaying)
            {
                _backgroundAudio.Stop();
            }
        }

        public bool IsPlayingSingleSound()
        {
            return _singleAudio.IsPlaying;
        }
        /// <summary>
        /// 播放单通道音效
        /// </summary>
        public async void PlaySingleSound(string clipName, bool isLoop = false, float speed = 1)
        {
            var clip = await res.LoadAsset<AudioClip>(clipName);
            if (_singleAudio.IsPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.SetParams(clip, isLoop, speed, clipName);
            _singleAudio.Play();
        }

        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        public void PauseSingleSound(bool isGradual = true)
        {
            if (!_singleAudio.IsPlaying) return;
            // if (isGradual)
            // {
            //     _singleAudio.DOFade(0, 2);
            // }
            // else
            {
                _singleAudio.Volume = 0;
            }
        }

        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        public void UnPauseSingleSound(bool isGradual = true)
        {
            if (!_singleAudio.IsPlaying) return;
            // if (isGradual)
            // {
            //     _singleAudio.DOFade(SoundEffectVolume, 2);
            // }
            // else
            {
                _singleAudio.Volume = SoundEffectVolume;
            }
        }

        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            if (_singleAudio.IsPlaying)
            {
                _singleAudio.Stop();
            }
        }

        /// <summary>
        /// 播放多通道音效
        /// </summary>
        public async void PlayMultipleSound(string clipName, bool isLoop = false, float speed = 1)
        {
            if(SoundEffectVolume<=0)return;
            var clip = await res.LoadAsset<AudioClip>(clipName);
            PlayMultipleSound(clip, isLoop, speed);
        }
        
        /// <summary>
        /// 播放多通道音效
        /// </summary>
        public void PlayMultipleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            var audio = ExtractIdleMultipleAudioSource();
            audio.SetParams(clip, isLoop, speed);
            audio.Play();
        }

        /// <summary>
        /// 停止播放指定的多通道音效
        /// </summary>
        public void StopMultipleSound(string name)
        {
            foreach (var audio in _multipleAudio)
            {
                if (!audio.IsPlaying) continue;
                if (audio.IsSameClip(name))
                {
                    audio.Stop();
                }
            }
        }

        /// <summary>
        /// 停止播放所有多通道音效
        /// </summary>
        public void StopAllMultipleSound()
        {
            foreach (var audio in _multipleAudio)
            {
                if (audio.IsPlaying)
                {
                    audio.Stop();
                }
            }
        }

        /// <summary>
        /// 销毁所有闲置中的多通道音效的音源
        /// </summary>
        public void ClearIdleMultipleAudioSource()
        {
            for (int i = 0; i < _multipleAudio.Count; i++)
            {
                if (_multipleAudio[i].IsPlaying) continue;
                AudioSourcePlayer player = _multipleAudio[i];
                _multipleAudio.RemoveAt(i);
                i -= 1;
                player.Dispose();
            }
        }

        /// <summary>
        /// 播放世界音效
        /// </summary>
        public async void PlayWorldSound(GameObject attachTarget, string clipName, bool isLoop = false, float speed = 1)
        {
            var clip = await res.LoadAsset<AudioClip>(clipName);
            if (_worldAudio.ContainsKey(attachTarget))
            {
                AudioSource audio = _worldAudio[attachTarget];
                if (audio.isPlaying)
                {
                    audio.Stop();
                }

                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.spatialBlend = 1;
                audio.Play();
            }
            else
            {
                AudioSource audio = AttachAudioSource(attachTarget, WorldPriority, SoundEffectVolume);
                _worldAudio.Add(attachTarget, audio);
                audio.clip = clip;
                audio.loop = isLoop;
                audio.pitch = speed;
                audio.spatialBlend = 1;
                audio.Play();
            }
        }

        /// <summary>
        /// 暂停播放指定的世界音效
        /// </summary>
        public void PauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (!_worldAudio.ContainsKey(attachTarget)) return;
            AudioSource audio = _worldAudio[attachTarget];
            if (!audio.isPlaying) return;
            // if (isGradual)
            // {
            //     audio.DOFade(0, 2);
            // }
            // else
            {
                audio.volume = 0;
            }
        }

        /// <summary>
        /// 恢复播放指定的世界音效
        /// </summary>
        public void UnPauseWorldSound(GameObject attachTarget, bool isGradual = true)
        {
            if (!_worldAudio.ContainsKey(attachTarget)) return;
            AudioSource audio = _worldAudio[attachTarget];
            if (!audio.isPlaying) return;
            // if (isGradual)
            // {
            //     audio.DOFade(SoundEffectVolume, 2);
            // }
            // else
            {
                audio.volume = SoundEffectVolume;
            }
        }

        /// <summary>
        /// 停止播放所有的世界音效
        /// </summary>
        public void StopAllWorldSound()
        {
            foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
            {
                if (audio.Value.isPlaying)
                {
                    audio.Value.Stop();
                }
            }
        }

        /// <summary>
        /// 销毁所有闲置中的世界音效的音源
        /// </summary>
        public void ClearIdleWorldAudioSource()
        {
            HashSet<GameObject> removeSet = new HashSet<GameObject>();
            foreach (KeyValuePair<GameObject, AudioSource> audio in _worldAudio)
            {
                if (!audio.Value.isPlaying)
                {
                    removeSet.Add(audio.Key);
                    Object.Destroy(audio.Value);
                }
            }

            foreach (GameObject item in removeSet)
            {
                _worldAudio.Remove(item);
            }
        }

        /// <summary>
        /// 创建一个音源
        /// </summary>
        private AudioSource CreateAudioSource(string name, int priority, float volume)
        {
            GameObject audioObj = new GameObject(name);
            audioObj.transform.SetParent(gameObject.transform);
            audioObj.transform.localPosition = Vector3.zero;
            audioObj.transform.localRotation = Quaternion.identity;
            audioObj.transform.localScale = Vector3.one;
            AudioSource audio = audioObj.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.mute = _isMute;
            return audio;
        }

        /// <summary>
        /// 创建一个音源包装类
        /// </summary>
        private AudioSourcePlayer CreateAudioSourcePlayer(string name, int priority, float volume)
        {
            var audio = CreateAudioSource(name, priority, volume);
            return new AudioSourcePlayer(audio);
        }

        /// <summary>
        /// 附加一个音源
        /// </summary>
        private AudioSource AttachAudioSource(GameObject target, int priority, float volume)
        {
            AudioSource audio = target.AddComponent<AudioSource>();
            audio.playOnAwake = false;
            audio.priority = priority;
            audio.volume = volume;
            audio.mute = _isMute;
            return audio;
        }

        /// <summary>
        /// 提取闲置中的多通道音源
        /// </summary>
        private AudioSourcePlayer ExtractIdleMultipleAudioSource()
        {
            foreach (var audioSource in _multipleAudio)
            {
                if (!audioSource.IsPlaying)
                {
                    return audioSource;
                }
            }

            var audio = CreateAudioSourcePlayer("MultipleAudio", MultiplePriority, SoundEffectVolume);
            _multipleAudio.Add(audio);
            return audio;
        }

        public override void Dispose()
        {
            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
        }
    }
}