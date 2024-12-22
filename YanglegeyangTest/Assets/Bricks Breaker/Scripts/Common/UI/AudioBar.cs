using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioBar : MonoBehaviour, IDragHandler, IPointerUpHandler
{


    public enum SliderType
    {
        Effect,
        Music
    }

    public SliderType sliderType;
    Slider slider;

    public Color colorMute;
    public Color colorOn;
    public Image audioIcon;
    public Image iconMude;


    public PopupPause _PopupPause;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        slider.value = Value;
        SetIocn();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.Instance.PlayEffect(SoundList.sound_common_btn_in);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (slider.value < 0)
        {
            slider.value = 0;
        }
        else if (slider.value > 1)
        {
            slider.value = 1;
        }

        Value = slider.value;
        if (sliderType == SliderType.Music)
        {
            SoundManager.Instance.SetBgmVolume();
        }
        else
        {
            SoundManager.Instance.SetEffectVolume();
        }

        SetIocn();
    }

    void SetIocn()
    {
        if (slider.value <= 0)
        {
            if (_PopupPause != null)
            {
                _PopupPause.SetMusicOnOff(false);
            }

            audioIcon.color = colorMute;
            iconMude.gameObject.SetActive(true);
        }
        else
        {
            if (_PopupPause != null)
            {
                _PopupPause.SetMusicOnOff(true);
            }

            iconMude.gameObject.SetActive(false);
            audioIcon.color = colorOn;
        }
    }

    float Value
    {
        get
        {
            if (sliderType == SliderType.Music)
            {
                return Data.VolumeMusic;
            }
            else
            {
                return Data.VolumeEffect;
            }
        }
        set
        {
            if (sliderType == SliderType.Music)
            {
                Data.VolumeMusic = value;
            }
            else
            {
                Data.VolumeEffect = value;
            }
        }
    }
}
