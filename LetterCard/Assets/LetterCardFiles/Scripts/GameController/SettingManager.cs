
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public static SettingManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    //下面是设置相关
    [Header("音量设置")]
    [SerializeField] private GameObject TurnOnSoundImage;
    [SerializeField] private GameObject TurnOffSoundImage;
    /// <summary>
    /// 切换音效开关
    /// </summary>
    public void ToggleSoundButton()
    {
        TurnOnSoundImage.SetActive(!TurnOnSoundImage.activeSelf);
        TurnOffSoundImage.SetActive(!TurnOffSoundImage.activeSelf);

        if (TurnOnSoundImage.activeSelf)
        {
            AudioManager.instance.SetSoundEffectsVolume(0.5f / 2);
            soundSlider.value = 0.5f;
        }
        else
        {
            AudioManager.instance.SetSoundEffectsVolume(0);
            soundSlider.value = 0;
        }

    }

    [SerializeField] private GameObject TurnOnMusicImage;
    [SerializeField] private GameObject TurnOffMusicImage;
    /// <summary>
    /// 切换音乐开关
    /// </summary>
    public void ToggleMusicButton()
    {
        TurnOnMusicImage.SetActive(!TurnOnMusicImage.activeSelf);
        TurnOffMusicImage.SetActive(!TurnOffMusicImage.activeSelf);

        if (TurnOnMusicImage.activeSelf)
        {
            AudioManager.instance.SetBackgroundMusicVolume(0.5f / 10);
            musicSlider.value = 0.5f;
        }
        else
        {
            AudioManager.instance.SetBackgroundMusicVolume(0);
            musicSlider.value = 0;
        }
    }

    [SerializeField] private Text soundValueText;
    [SerializeField] private Slider soundSlider;
    /// <summary>
    /// 音效slider
    /// </summary>
    public void OnSoundSliderChange()
    {
        //切换图标状态
        TurnOnSoundImage.SetActive(soundSlider.value != 0);
        TurnOffSoundImage.SetActive(soundSlider.value == 0);

        soundValueText.text = ((int)(100 * soundSlider.value)).ToString();
        AudioManager.instance.SetSoundEffectsVolume(soundSlider.value / 2);
    }

    [SerializeField] private Text musicValueText;
    [SerializeField] private Slider musicSlider;
    /// <summary>
    /// 音乐slider
    /// </summary>
    public void OnMusicSliderChange()
    {
        //切换图标状态
        TurnOnMusicImage.SetActive(musicSlider.value != 0);
        TurnOffMusicImage.SetActive(musicSlider.value == 0);

        musicValueText.text = ((int)(100 * musicSlider.value)).ToString();
        AudioManager.instance.SetBackgroundMusicVolume(musicSlider.value / 10);
    }

    /// <summary>
    /// 关闭当前面板
    /// </summary>
    public void CloseCurrentPanel()
    {
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 打开当前面板
    /// </summary>
    public void OpenCurrentPanel()
    {
        GetComponent<Canvas>().enabled = true;
    }
}
