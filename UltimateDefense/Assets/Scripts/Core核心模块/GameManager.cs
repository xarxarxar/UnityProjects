using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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

    //每三分钟向服务器提交一次在线时长的统计
    private IEnumerator TrackOnlineTime()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(18f); // 等待180秒
            APIAccess.Instance.UpdateTodayOnlineMinutes(DataManager.Instance.UserID, onSuccess: (res) =>
            {
                DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value = (int)MathF.Min(res.data["TodayOnlineMinutes"].Value<int>(),60) ;
                Debug.Log("在线时间 +3 分钟，总在线分钟：" + DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value);
            });
            
        }
    }

    #endregion


}
