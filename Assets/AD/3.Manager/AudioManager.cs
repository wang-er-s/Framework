/*
* Create by Soso
* Time : 2018-12-11-03 下午
*/
using UnityEngine;
using System;
using System.Collections.Generic;

namespace AD
{
    public class AudioManager : MonoBehaviour
    {
        private class SoundObj
        {
            public string clipName;

            public AudioSource sourceClip;

            public bool isPlaying;

            public SoundObj PlaySound ( Vector3 pos, string soundName, bool is3D = false )
            {
                if ( sourceClip == null )
                {
                    sourceClip = new GameObject ( "SoundObj" ).GetOrAddComponent<AudioSource> ();
                    DontDestroyOnLoad ( sourceClip.gameObject );
                }

                if ( clipName != soundName )
                {
                    sourceClip.clip = ResLoader.Load<AudioClip> (  soundName, isCache : true );
                    clipName        = soundName;
                }

                sourceClip.transform.Position ( pos );
                if ( is3D ) sourceClip.spatialBlend = 1;
                else sourceClip.spatialBlend        = 0;
                sourceClip.Play ();
                isPlaying = true;
                TimerManager.Instance.AddTimerEvent ( sourceClip.clip.length, () => isPlaying = false );
                return this;
            }
        }

        private void Awake ()
        {
            soundList = new List<SoundObj> ();
        }

        private List<SoundObj> soundList;

        public AudioSource Music;

        public void PlayMusic ( string musicName )
        {
            if ( !Music )
            {
                Music = new GameObject ( musicName ).transform.Parent ( transform ).gameObject.
                                                     AddComponent<AudioSource> ();
                Music.clip = ResLoader.Load<AudioClip> (  musicName, isCache : true );
            }
            else if ( musicName != Music.clip.name )
            {
                Music.clip = ResLoader.Load<AudioClip> (  musicName, isCache : true );
            }

            Music.loop = true;
            Music.Play ();
        }

        public void PlayerSound ( GameObject target, string soundName, bool is3D = false )
        {
            foreach ( var item in soundList )
            {
                if ( !item.isPlaying )
                {
                    item.PlaySound ( target.transform.position, soundName, is3D );
                    return;
                }
            }

            soundList.Add ( new SoundObj ().PlaySound ( target.transform.position, soundName, is3D ) );
        }



        public void StopMusic ()
        {
            if ( Music )
                Music.Stop ();
        }

        public void StopSound ()
        {
            foreach ( var item in soundList )
            {
                item.sourceClip.Stop ();
                item.isPlaying = false;
            }
        }

        public void OpenOrCloseMusic ( bool isOpen )
        {
            if ( isOpen && Music )
                Music.Play ();
            else
                StopMusic ();
        }

        public void OpenOrCloseSound ( bool isOpen )
        {
            if ( !isOpen )
                StopSound ();
        }
    }
}
