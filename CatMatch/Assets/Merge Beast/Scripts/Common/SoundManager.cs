using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MergeBeast
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager _instance;
        public static SoundManager Instance => _instance;

        [SerializeField] private AudioSource _myAudio;
        [SerializeField] private AudioSource _musicBG;
        [SerializeField] private SoundDictionary _dictionSound;

        private bool _soundIsOn;
        private bool _musicIsOn;
        public bool SoundIsOn
        {
            get { return _soundIsOn; }
        }

        public bool MusicIsOn
        {
            get { return _musicIsOn; }
        }

        // Use this for initialization
        void Awake()
        {
            if (_instance == null) _instance = this;

            _soundIsOn = PlayerPrefs.GetInt(StringDefine.SOUND_KEY, 0) == 0;
            float volum = PlayerPrefs.GetFloat(StringDefine.MUSIC_KEY, 1);
            _musicIsOn = volum > 0;
            _musicBG.volume = volum;
        }


        public void UpdateSliderMusic(float volum)
        {
            _musicBG.volume = volum;
            PlayerPrefs.SetFloat(StringDefine.MUSIC_KEY, volum);
            _musicIsOn = volum > 0;
        }

        public void UpdateSound(bool isOn)
        {
            _soundIsOn = isOn;
            PlayerPrefs.SetInt(StringDefine.SOUND_KEY, isOn ? 0 : 1);
        }



        public void PlaySound(EnumDefine.SOUND sound)
        {
            if (!_soundIsOn || CPlayer.OutMainGame) return;

            if (!_dictionSound.ContainsKey(sound)) return;
            if (sound == EnumDefine.SOUND.BEAST_MERGE)
            {
                if (!_myAudio.isPlaying)
                    _myAudio.Play();
            }
            else
            {
                _myAudio.PlayOneShot(_dictionSound[sound]);
            }

        }

        public void MuteMainGame(bool mute)
        {
            if (SoundIsOn)
            {
                CPlayer.OutMainGame = mute;
                _myAudio.volume = mute ? 0 : 1;
            }
            if (MusicIsOn) _musicBG.volume = mute ? 0 : 1;
        }
    }
}
