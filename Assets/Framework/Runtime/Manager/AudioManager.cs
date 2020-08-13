using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Manager
{
    public sealed class AudioManager : IManager
    {
        #region 公开属性

        /// <summary>
        /// 背景音乐优先级
        /// </summary>
        public int BackgroundPriority = 0;

        /// <summary>
        /// 单通道音效优先级
        /// </summary>
        public int SinglePriority = 10;

        /// <summary>
        /// 多通道音效优先级
        /// </summary>
        public int MultiplePriority = 20;

        /// <summary>
        /// 世界音效优先级
        /// </summary>
        public int WorldPriority = 30;

        /// <summary>
        /// 背景音乐音量
        /// </summary>
        public float BackgroundVolume = 0.6f;

        /// <summary>
        /// 单通道音效音量
        /// </summary>
        public float SingleVolume = 1;

        /// <summary>
        /// 多通道音效音量
        /// </summary>
        public float MultipleVolume = 1;

        /// <summary>
        /// 世界音效音量
        /// </summary>
        public float WorldVolume = 1;


        #endregion

        #region 私有属性

        private AudioSource _backgroundAudio;
        private AudioSource _singleAudio;
        private readonly List<AudioSourcePlayer> _multipleAudio = new List<AudioSourcePlayer>();
        private readonly Dictionary<GameObject, AudioSource> _worldAudio = new Dictionary<GameObject, AudioSource>();
        private bool _isMute;
        private readonly GameObject _gameObject;

        public AudioManager(GameObject audioGameObject)
        {
            this._gameObject = audioGameObject;
        }

        #endregion

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

            public void SetParams(AudioClip clip, bool isLoop, float speed)
            {
                _audioSource.clip = clip;
                _audioSource.loop = isLoop;
                _audioSource.pitch = speed;
                _audioSource.spatialBlend = 0;
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

        public static AudioManager Instance;

        void IManager.Init(params object[] para)
        {
            Instance = this;
            _backgroundAudio = CreateAudioSource("BackgroundAudio", BackgroundPriority, BackgroundVolume);
            _singleAudio = CreateAudioSource("SingleAudio", SinglePriority, SingleVolume);
        }

        void IManager.Update(float deltaTime)
        {

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
                _backgroundAudio.mute = value;
                _singleAudio.mute = value;
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
        public void PlayBackgroundMusic(AudioClip clip, bool isLoop = true, float speed = 1)
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }

            _backgroundAudio.clip = clip;
            _backgroundAudio.loop = isLoop;
            _backgroundAudio.pitch = speed;
            _backgroundAudio.spatialBlend = 0;
            _backgroundAudio.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        public void PauseBackgroundMusic(bool isGradual = true)
        {
            if (!_backgroundAudio.isPlaying) return;
            if (isGradual)
            {
                _backgroundAudio.DOFade(0, 2);
            }
            else
            {
                _backgroundAudio.volume = 0;
            }
        }

        /// <summary>
        /// 恢复播放背景音乐
        /// </summary>
        public void UnPauseBackgroundMusic(bool isGradual = true)
        {
            if (!_backgroundAudio.isPlaying) return;
            if (isGradual)
            {
                _backgroundAudio.DOFade(BackgroundVolume, 2);
            }
            else
            {
                _backgroundAudio.volume = BackgroundVolume;
            }
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (_backgroundAudio.isPlaying)
            {
                _backgroundAudio.Stop();
            }
        }

        /// <summary>
        /// 播放单通道音效
        /// </summary>
        public void PlaySingleSound(AudioClip clip, bool isLoop = false, float speed = 1)
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }

            _singleAudio.clip = clip;
            _singleAudio.loop = isLoop;
            _singleAudio.pitch = speed;
            _singleAudio.spatialBlend = 0;
            _singleAudio.Play();
        }

        /// <summary>
        /// 暂停播放单通道音效
        /// </summary>
        public void PauseSingleSound(bool isGradual = true)
        {
            if (!_singleAudio.isPlaying) return;
            if (isGradual)
            {
                _singleAudio.DOFade(0, 2);
            }
            else
            {
                _singleAudio.volume = 0;
            }
        }

        /// <summary>
        /// 恢复播放单通道音效
        /// </summary>
        public void UnPauseSingleSound(bool isGradual = true)
        {
            if (!_singleAudio.isPlaying) return;
            if (isGradual)
            {
                _singleAudio.DOFade(SingleVolume, 2);
            }
            else
            {
                _singleAudio.volume = SingleVolume;
            }
        }

        /// <summary>
        /// 停止播放单通道音效
        /// </summary>
        public void StopSingleSound()
        {
            if (_singleAudio.isPlaying)
            {
                _singleAudio.Stop();
            }
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
        public void PlayWorldSound(GameObject attachTarget, AudioClip clip, bool isLoop = false, float speed = 1)
        {
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
                AudioSource audio = AttachAudioSource(attachTarget, WorldPriority, WorldVolume);
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
            if (isGradual)
            {
                audio.DOFade(0, 2);
            }
            else
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
            if (isGradual)
            {
                audio.DOFade(WorldVolume, 2);
            }
            else
            {
                audio.volume = WorldVolume;
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
            audioObj.transform.SetParent(_gameObject.transform);
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

            var audio = CreateAudioSourcePlayer("MultipleAudio", MultiplePriority, MultipleVolume);
            _multipleAudio.Add(audio);
            return audio;
        }

        public void Dispose()
        {
            StopBackgroundMusic();
            StopSingleSound();
            StopAllMultipleSound();
            StopAllWorldSound();
        }
    }
}