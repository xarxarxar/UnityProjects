
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            WechatManager.CallSetUserData(new PlayerInfo());
        }
    }

    /// <summary>
    /// 获取玩家信息
    /// </summary>
    /// <returns></returns>
    public PlayerInfo DownloadPlayerInfo()
    {
        return null;
    }

    /// <summary>
    /// 上传玩家信息
    /// </summary>
    public void UploadPlayerInfo()
    {
        WechatManager.CallSetUserData(new PlayerInfo());
    }
}
