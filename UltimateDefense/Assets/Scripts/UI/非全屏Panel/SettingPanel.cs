using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    [SerializeField] private Toggle _isTurnOnDamageTextToggle;//是否打开伤害文字
    [SerializeField] private Toggle _isTurnOnMusicToggle;//是否打开音乐
    [SerializeField] private Toggle _isTurnOnSoundToggle;//是否打开音效

    [SerializeField] private GameObject _activeDamageText;//伤害文字的active的物体
    [SerializeField] private GameObject _activeTurnOnMusic;//打开音乐的active的物体
    [SerializeField] private GameObject _activeTurnOnSound;//打开音效的active的物体

    //[SerializeField] private GameObject _musicSlider;//控制音乐大小的slider
    //[SerializeField] private GameObject _soundSlider;//控制音乐大小的slider

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
        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("DamageText"))
        {
            DataManager.Instance.PlayerInfo.Config["DamageText"] = 0;//0表示false
            DataManager.Instance.SavePlayerInfo();
        }
        _isTurnOnDamageTextToggle.isOn = DataManager.Instance.PlayerInfo.Config["DamageText"] == 1;
        _activeDamageText.SetActive(DataManager.Instance.PlayerInfo.Config["DamageText"]==1);

        //是否打开震动
        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("Vibrate"))
        {
            DataManager.Instance.PlayerInfo.Config["Vibrate"] = 1;//1表示true
            DataManager.Instance.SavePlayerInfo();
        }


        //是否打开音乐
        //音乐的音量
        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("MusicVolum"))
        {
            DataManager.Instance.PlayerInfo.Config["MusicVolum"] = 50;
            DataManager.Instance.SavePlayerInfo();
        }
        _isTurnOnMusicToggle.isOn = DataManager.Instance.PlayerInfo.Config["MusicVolum"] > 0;
        _activeTurnOnMusic.SetActive(DataManager.Instance.PlayerInfo.Config["MusicVolum"] >0);
        //_musicVolumText.text = DataManager.Instance.PlayerInfo.Config["MusicVolum"].ToString();
        _musicVolumSlider.value = DataManager.Instance.PlayerInfo.Config["MusicVolum"];


        //是否打开音效
        //音效的音量
        if (!DataManager.Instance.PlayerInfo.Config.ContainsKey("SoundVolum"))
        {
            DataManager.Instance.PlayerInfo.Config["SoundVolum"] = 50;
            DataManager.Instance.SavePlayerInfo();
        }
        _isTurnOnSoundToggle.isOn = DataManager.Instance.PlayerInfo.Config["SoundVolum"] > 0;
        _activeTurnOnSound.SetActive(DataManager.Instance.PlayerInfo.Config["SoundVolum"] >0);
        //_soundVolumText.text = DataManager.Instance.PlayerInfo.Config["SoundVolum"].ToString();
        _soundVolumSlider.value = DataManager.Instance.PlayerInfo.Config["SoundVolum"];

    }

    //为组件添加事件
    private void AddEvent()
    {
        _isTurnOnDamageTextToggle.onValueChanged.AddListener(isOn =>
        {
            _activeDamageText.SetActive(isOn);
            DataManager.Instance.PlayerInfo.Config["DamageText"] = isOn ? 1 : 0;
            DataManager.Instance.SavePlayerInfo();
        });

        _isTurnOnMusicToggle.onValueChanged.AddListener(isOn =>
        {
            DataManager.Instance.PlayerInfo.Config["MusicVolum"] =isOn ? 50 : 0;
            _musicVolumSlider.value = DataManager.Instance.PlayerInfo.Config["MusicVolum"];
            _activeTurnOnMusic.SetActive(isOn);
            DataManager.Instance.SavePlayerInfo();
        });

        _isTurnOnSoundToggle.onValueChanged.AddListener(isOn =>
        {
            DataManager.Instance.PlayerInfo.Config["SoundVolum"] = isOn ? 50 : 0;
            _soundVolumSlider.value = DataManager.Instance.PlayerInfo.Config["SoundVolum"];
            _activeTurnOnSound.SetActive(isOn);
            DataManager.Instance.SavePlayerInfo();

        });

        _musicVolumSlider.onValueChanged.AddListener(value =>
        {
            _musicVolumText.text = value.ToString();
            DataManager.Instance.PlayerInfo.Config["MusicVolum"] = value;
            DataManager.Instance.SavePlayerInfo();
        });

        _soundVolumSlider.onValueChanged.AddListener(value =>
        {
            _soundVolumText.text = value.ToString();
            DataManager.Instance.PlayerInfo.Config["SoundVolum"]=value;
            DataManager.Instance.SavePlayerInfo();
        });
    }


    private void OnDisable()
    {
        _isTurnOnDamageTextToggle.onValueChanged.RemoveAllListeners();
        _isTurnOnMusicToggle.onValueChanged.RemoveAllListeners();
        _isTurnOnSoundToggle.onValueChanged.RemoveAllListeners();

        _musicVolumSlider.onValueChanged.RemoveAllListeners();
        _soundVolumSlider.onValueChanged.RemoveAllListeners();

    }
    //改变了声音状态，包括震动和声音
    private void ChangeAudioState()
    {

    }

}
