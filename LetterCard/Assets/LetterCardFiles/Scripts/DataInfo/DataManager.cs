
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
            AudioManager.instance.MusicVolume=globalPlayerInfo.musicVolume;
            AudioManager.instance.SoundVolume=globalPlayerInfo.soundEffectVolume;

            callback?.Invoke();
        });
    }

    /// <summary>
    /// 上传玩家信息
    /// </summary>
    public void UploadPlayerInfo()
    {
        globalPlayerInfo.musicVolume = AudioManager.instance.MusicVolume; 
        globalPlayerInfo.soundEffectVolume = AudioManager.instance.SoundVolume;
        WechatManager.CallSetUserData(globalPlayerInfo);
    }
}
