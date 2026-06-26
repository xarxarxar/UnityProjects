namespace EKStudio
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    
    public class SettingsManager : MonoBehaviour
    {
        public Button soundButton, hapticButton;
        public GameObject soundOn, soundOff, hapticOn, hapticOff;
    
    
        private void Awake()
        {
            soundButton.onClick.AddListener(SoundChange);
            hapticButton.onClick.AddListener(HapticChange);
    
            AllChange();
        }
    
        public void SoundChange()
        {
            PlayerPrefs.SetInt("SoundButton", soundOff.activeSelf ? 0 : 1);
            AllChange();
        }
    
        public void HapticChange()
        {
            PlayerPrefs.SetInt("HapticButton", hapticOff.activeSelf ? 0 : 1);
            AllChange();
        }
    
        void AllChange()
        {
    
            if (PlayerPrefs.GetInt("SoundButton") == 0)
            {
                AudioListener.volume = 1;
                soundOn.SetActive(true);
                soundOff.SetActive(false);
            }
            else
            {
                AudioListener.volume = 0;
                soundOn.SetActive(false);
                soundOff.SetActive(true);
            }
    
            if (PlayerPrefs.GetInt("HapticButton") == 0)
            {
                PlayerPrefs.SetInt("IsHapticOpen", 1);
                hapticOn.SetActive(true);
                hapticOff.SetActive(false);
            }
            else
            {
                PlayerPrefs.SetInt("IsHapticOpen", 0);
                hapticOn.SetActive(false);
                hapticOff.SetActive(true);
            }
                
        }
    }
    
}
