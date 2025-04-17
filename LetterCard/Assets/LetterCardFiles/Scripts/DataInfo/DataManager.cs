
using UnityEngine;
using UnityEngine.Events;

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
    public void DownloadPlayerInfo(UnityAction callback)
    {
        WechatManager.GetUserData((playerInfo) =>
        {
            globalPlayerInfo= playerInfo;
            //GameEntrance.instance.CoinCount =globalPlayerInfo.coinCount;
            AudioManager.instance.MusicVolume=globalPlayerInfo.settings.musicVolume;
            AudioManager.instance.SoundVolume=globalPlayerInfo.settings.soundEffectVolume;

            callback?.Invoke();
        });
    }

    /// <summary>
    /// 上传玩家信息
    /// </summary>
    public void UploadPlayerInfo()
    {
        //globalPlayerInfo.coinCount =(int) GameEntrance.instance.CoinCount;
        globalPlayerInfo.settings = new GameSettings(AudioManager.instance.MusicVolume, AudioManager.instance.SoundVolume);
        WechatManager.CallSetUserData(globalPlayerInfo);
    }
}
