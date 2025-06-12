/****************************************************
    功能：音频管理器
    作者：ZH
    创建日期：#2025/01/09#
    修改人：ZH
    修改日期：#2025/01/14#
    修改内容：
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voltage
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private const int maxSoundNum = 2;

        private AudioSource music;
        private AudioSource[] sounds = new AudioSource[maxSoundNum];

        private int isMusicMute;
        private int isSoundMute;

        private float musicVolume;
        private float soundVolume;

        private int nowSoundId = 0;

        public void Init()
        {
            isMusicMute = PlayerPrefs.GetInt("isMusicMute", 0);
            isSoundMute = PlayerPrefs.GetInt("isSoundMute", 0);
            musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
            soundVolume = PlayerPrefs.GetFloat("soundVolume", 1.0f);

            music = gameObject.AddComponent<AudioSource>();
            music.mute = isMusicMute == 1;
            music.volume = musicVolume;

            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i] = gameObject.AddComponent<AudioSource>();
                sounds[i].mute = isSoundMute == 1;
                sounds[i].volume = soundVolume;
            }
        }

        public void PlayMusic(string url, bool loop = true)
        {
            AudioClip clip = ResManager.Instance.LoadAudio(url);
            music.clip = clip;
            music.loop = loop;
            music.Play();
        }

        public int PlaySound(string url, bool loop = false)
        {
            int soundid = nowSoundId;
            AudioClip clip = ResManager.Instance.LoadAudio(url);
            sounds[nowSoundId].clip = clip;
            sounds[nowSoundId].loop = loop;
            sounds[nowSoundId].Play();

            nowSoundId++;
            nowSoundId = (nowSoundId >= sounds.Length) ? 0 : nowSoundId;
            return soundid;
        }

        public void StopMusic()
        {
            music.Stop();
            music.clip = null;
        }

        public void StopSound(int soundid)
        {
            if (soundid < 0 || soundid >= sounds.Length) return;

            sounds[soundid].Stop();
            sounds[nowSoundId].clip = null;
        }

        public void StopAllSounds()
        {
            for (int i = 0; i < sounds.Length; i++)
            {
                sounds[i].Stop();
                sounds[i].clip = null;
            }
        }

        public void SetMusicMute(bool mute)
        {
            if (mute == (isMusicMute == 1)) return;

            isMusicMute = mute ? 1 : 0;
            music.mute = mute;

            PlayerPrefs.SetInt("isMusicMute", isMusicMute);
        }

        public void SetSoundMute(bool mute)
        {
            if (mute == (isSoundMute == 1)) return;

            isSoundMute = mute ? 1 : 0;
            for (int i = 0; i < maxSoundNum; i++)
            {
                sounds[i].mute = mute;
            }

            PlayerPrefs.SetInt("isSoundMute", isSoundMute);
        }

        public void SetMusicVolume(float value)
        {
            if (value < 0.0f || value > 1.0f) return;

            musicVolume = value;
            music.volume = musicVolume;
            PlayerPrefs.SetFloat("musicVolume", musicVolume);
        }

        public void SetSoundVolume(float value)
        {
            if (value < 0.0f || value > 1.0f) return;

            soundVolume = value;
            for (int i = 0; i < maxSoundNum; i++)
            {
                sounds[i].volume = value;
            }

            PlayerPrefs.SetFloat("soundVolume", soundVolume);
        }

        public bool IsMusicMuted
        {
            get
            {
                return isMusicMute == 1;
            }
        }

        public bool IsSoundMuted
        {
            get
            {
                return isSoundMute == 1;
            }
        }

        public float MusicVolume
        {
            get
            {
                return musicVolume;
            }
        }

        public float SoundVolume
        {
            get
            {
                return soundVolume;
            }
        }
    }
}
