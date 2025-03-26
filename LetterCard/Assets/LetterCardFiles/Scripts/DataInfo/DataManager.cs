
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public PlayerInfo globalPlayerInfo=new PlayerInfo();

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 获取玩家信息
    /// </summary>
    /// <returns></returns>
    public void DownloadPlayerInfo()
    {
        WechatManager.GetUserData((playerInfo) =>
        {
            globalPlayerInfo= playerInfo;
            GameEntrance.instance.CoinCount =globalPlayerInfo.coinCount;
            AudioManager.instance.SetBackgroundMusicVolume(globalPlayerInfo.settings.musicVolume);
            AudioManager.instance.SetSoundEffectsVolume(globalPlayerInfo.settings.soundEffectVolume);
        });
    }

    /// <summary>
    /// 上传玩家信息
    /// </summary>
    public void UploadPlayerInfo()
    {
        globalPlayerInfo.coinCount =(int) GameEntrance.instance.CoinCount;
        globalPlayerInfo.settings = new GameSettings(AudioManager.instance.GetBackgroundMusicVolume(), AudioManager.instance.GetSoundEffectsVolume());
        WechatManager.CallSetUserData(globalPlayerInfo);
    }
}
