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
        StartCoroutine(OnDayRefresh());//开始等待下一天的刷新
    }

    #endregion

    #region 私有方法

    //计算在线时长
    private IEnumerator TrackOnlineTime()
    {

        while (true)
        {
            yield return new WaitForSeconds(10f); // 等待60秒
            DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value += 1;
            DataManager.Instance.SavePlayerInfo();
            Debug.Log("在线时间 +1 分钟，总在线分钟：" + DataManager.Instance.PlayerInfo.TodayOnlineMinutes.Value);
        }
    }

    /// <summary>
    /// 日期刷新的时候
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnDayRefresh()
    {
        DateTime dateTimeNow = DateTime.Now;//此刻
        // 先得到“明天 0 点”
        DateTime tomorrowZero = dateTimeNow.Date.AddDays(1);
        // 计算剩余秒数
        float secondsLeft = (float)(tomorrowZero - dateTimeNow).TotalSeconds;
        yield return new WaitForSecondsRealtime(secondsLeft);
        DataManager.Instance.RefreshDailyPlayerInfo();
    }
    #endregion


}
