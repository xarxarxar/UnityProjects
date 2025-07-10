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

    [SerializeField] private GameObject _musicSlider;//控制音乐大小的slider
    [SerializeField] private GameObject _soundSlider;//控制音乐大小的slider

    [SerializeField] private Text _musicVolumText;//音乐大小文字
    [SerializeField] private Text _soundVolumText;//音效大小文字

    [SerializeField] private Slider _musicVolumSlider;//音乐大小的slider
    [SerializeField] private Slider _soundVolumSlider;//音效大小的slider


    public override void OnEnable()
    {
        base.OnEnable();
        _isTurnOnDamageTextToggle.onValueChanged.AddListener(isOn =>
        {
            _activeDamageText.SetActive(isOn);
        });

        _isTurnOnMusicToggle.onValueChanged.AddListener(isOn =>
        {
            _activeTurnOnMusic.SetActive(isOn);
            _musicSlider.SetActive(isOn);
        });

        _isTurnOnSoundToggle.onValueChanged.AddListener(isOn =>
        {
            _activeTurnOnSound.SetActive(isOn);
            _soundSlider.SetActive(isOn);
        });

        _musicVolumSlider.onValueChanged.AddListener(value => 
        {
            _musicVolumText.text=value.ToString();
        });

        _soundVolumSlider.onValueChanged.AddListener(value =>
        {
            _soundVolumText.text = value.ToString();
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

}
