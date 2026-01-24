using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    [SerializeField] private Toggle _isTurnOnDamageTextToggle;//是否打开伤害文字
    [SerializeField] private Toggle _isTurnOnVibrate;//是否打开震动

    [SerializeField] private GameObject _activeDamageText;//伤害文字的active的物体
    [SerializeField] private GameObject _inactiveDamageText;//伤害文字的inactive的物体
    [SerializeField] private GameObject _activeTurnOnVibrate;//打开震动的active的物体
    [SerializeField] private GameObject _inactiveTurnOnVibrate;//打开震动的inactive的物体


    [SerializeField] private Text _musicVolumText;//音乐大小文字
    [SerializeField] private Text _soundVolumText;//音效大小文字

    [SerializeField] private Slider _musicVolumSlider;//音乐大小的slider
    [SerializeField] private Slider _soundVolumSlider;//音效大小的slider


    public override void OnEnable()
    {
        base.OnEnable();
        AddEvent();
        InitValue();
    }

    //初始化值
    private void InitValue()
    {
        //是否打开伤害文字
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("DamageText", out var value))
        {
            DataManager.Instance.PlayerInfo.SetConfig("DamageText", 0); // 不存在 → 默认 false
        }
        _isTurnOnDamageTextToggle.isOn = DataManager.Instance.PlayerInfo.Config["DamageText"] == 1;
        _activeDamageText.SetActive(DataManager.Instance.PlayerInfo.Config["DamageText"]==1);
        _inactiveDamageText.SetActive(DataManager.Instance.PlayerInfo.Config["DamageText"]!=1);

        //是否打开震动
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("Vibrate", out var Vibratevalue))
        {
            DataManager.Instance.PlayerInfo.SetConfig("Vibrate", 1); //1表示true
        }
        _isTurnOnVibrate.isOn = DataManager.Instance.PlayerInfo.Config["Vibrate"] == 1;
        _activeTurnOnVibrate.SetActive(DataManager.Instance.PlayerInfo.Config["Vibrate"] == 1);
        _inactiveTurnOnVibrate.SetActive(DataManager.Instance.PlayerInfo.Config["Vibrate"] != 1);

        //是否打开音乐
        //音乐的音量
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("MusicVolum", out var MusicVolumvalue))
        {
            DataManager.Instance.PlayerInfo.SetConfig("MusicVolum", 50); // 不存在 → 默认 false
        }
        //_musicVolumText.text = DataManager.Instance.PlayerInfo.Config["MusicVolum"].ToString();
        _musicVolumSlider.value = DataManager.Instance.PlayerInfo.Config["MusicVolum"];


        //是否打开音效
        //音效的音量
        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("SoundVolum", out var SoundVolumvalue))
        {
            DataManager.Instance.PlayerInfo.SetConfig("SoundVolum", 50); // 不存在 → 默认 false
        }
        _soundVolumSlider.value = DataManager.Instance.PlayerInfo.Config["SoundVolum"];

    }

    //为组件添加事件
    private void AddEvent()
    {
        _isTurnOnDamageTextToggle.onValueChanged.AddListener(isOn =>
        {
            _activeDamageText.SetActive(isOn);
            _inactiveDamageText.SetActive(!isOn);
            DataManager.Instance.PlayerInfo.SetConfig("DamageText", isOn ? 1 : 0);
        });

        _isTurnOnVibrate.onValueChanged.AddListener(isOn =>
        {
            _activeTurnOnVibrate.SetActive(isOn);
            _inactiveTurnOnVibrate.SetActive(!isOn);
            DataManager.Instance.PlayerInfo.SetConfig("Vibrate", isOn ? 1 : 0);
        });

        _musicVolumSlider.onValueChanged.AddListener(value =>
        {
            _musicVolumText.text = value.ToString();
            DataManager.Instance.PlayerInfo.SetConfig("MusicVolum",value);
            AudioManager.Instance.SetBGMVolume(DataManager.Instance.PlayerInfo.Config["MusicVolum"] * 0.6f / 100);
        });

        _soundVolumSlider.onValueChanged.AddListener(value =>
        {
            _soundVolumText.text = value.ToString();
            DataManager.Instance.PlayerInfo.SetConfig("SoundVolum", value);
            AudioManager.Instance.SetSFXVolume(DataManager.Instance.PlayerInfo.Config["SoundVolum"] / 100); 
        });
    }


    private void OnDisable()
    {
        _isTurnOnDamageTextToggle.onValueChanged.RemoveAllListeners();

        _musicVolumSlider.onValueChanged.RemoveAllListeners();
        _soundVolumSlider.onValueChanged.RemoveAllListeners();

    }
    //改变了声音状态，包括震动和声音
    private void ChangeAudioState()
    {

    }

}
