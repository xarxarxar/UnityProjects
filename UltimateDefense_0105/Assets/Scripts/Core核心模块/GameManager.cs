using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 控制整个的游戏流程，游戏的入口
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;//单例
    /// <summary>
    /// GameManger单例
    /// </summary>
    public static GameManager Instance { get => _instance;}


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 游戏启动
    /// </summary>
    private void Start()
    {
        Init();
    }

    #region 公共方法
    /// <summary>
    /// 初始化游戏
    /// </summary>
    public void Init()
    {
        ManagerRegistry.InitManagers(InitStage.OutBattle);//初始化所有局外的Manager
        StartCoroutine(TrackOnlineTime());//开始在线时长的统计


    }

    #endregion

    #region 私有方法

    //每1分钟更新一次在线时长
    private IEnumerator TrackOnlineTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(60f); // 等待60秒
            DataManager.Instance.PlayerInfo.DailyTask.SetOnlineMinutes((int)MathF.Min(DataManager.Instance.PlayerInfo.DailyTask.OnlineMinutes+1, 60));//最多记录60分钟
            DataManager.Instance.SavePlayerInfoCloud();//每分钟上传一次
            Debug.Log("在线时间 +1 分钟，总在线分钟：" + DataManager.Instance.PlayerInfo.DailyTask.OnlineMinutes);
        }
    }

    #endregion


}
