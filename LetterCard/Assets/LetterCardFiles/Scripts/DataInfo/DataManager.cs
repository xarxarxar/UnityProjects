
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
        });
    }

    /// <summary>
    /// 上传玩家信息
    /// </summary>
    public void UploadPlayerInfo()
    {
        WechatManager.CallSetUserData(globalPlayerInfo);
    }
}
