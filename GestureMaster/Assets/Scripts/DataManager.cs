using UnityEngine;
using UnityEngine.Events;
using WeChatWASM;
using Newtonsoft.Json;
using System;

public  class TimeStamp
{
    public long timestamp;
}

/// <summary>
/// 管理数据的同步与上传
/// </summary>
public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    /// <summary>
    /// 获取服务器时间完毕
    /// </summary>
    public static event UnityAction<DateTime> OnGetDatetime;
    /// <summary>
    /// 玩家数据获取完毕
    /// </summary>
    public static event UnityAction OnGetGameInfo;
    private GameInfo PlayerInfo=>GameEntrance.instance.PlayerInfo;


    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        OpenAutoSaveGameInfo();//开启自动保存
    }

    /// <summary>
    /// 从云函数获取当前时间戳，精确到秒
    /// </summary>
    public void GetCurrentTime(UnityAction<long> callback)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "GetCurrentTime",
            success = (res) =>
            {
                TimeStamp timestamp= JsonConvert.DeserializeObject<TimeStamp>(res.result.ToString());
                callback?.Invoke(timestamp.timestamp);
                OnGetDatetime?.Invoke(TimestampToDateTime(timestamp.timestamp));
            },
            fail = (res) =>
            {

            },
            complete = (res) =>
            {

            }
        });
    }

    /// <summary>
    /// 保存数据到云端
    /// </summary>
    public void UploadGameInfo(GameInfo gameInfo)
    {
        UploadGameInfoFromWechat(gameInfo);
    }

    /// <summary>
    /// 下载用户数据
    /// </summary>
    /// <returns></returns>
    public void DownloadGameInfo(UnityAction<GameInfo> successAction)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "DownloadGameInfo",
            success = (res) =>
            {
                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    //CloudResponse response = JsonUtility.FromJson<CloudResponse>(res.result);
                    CloudResponse response = JsonConvert.DeserializeObject<CloudResponse>(res.result.ToString());
                    // 再提取实际数据
                    GameInfo localUserData = response.data;
                    successAction?.Invoke(localUserData);
                    OnGetGameInfo?.Invoke();
                }
            },
            fail = (res) =>
            {
                Debug.LogError("获取玩家数据失败：" + res.errMsg);
                TipManager.instance.ShowTip("获取数据失败");
            },
            complete = (res) =>
            {
                Debug.Log("获取玩家数据操作完成");
            }
        });
    }

    /// <summary>
    /// 供外部使用，同步数据
    /// </summary>
    public void SyncGameInfo()
    {
        string timeStr = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        SyncDecalStatusFromDatabase();//同步皮肤数据
        UploadGameInfo(PlayerInfo);
    }

    /// <summary>
    /// 开启自动保存玩家信息
    /// </summary>
    private void OpenAutoSaveGameInfo()
    {
        AutoSaveGameInfoWeChat();
    }

    //微信上传用户数据
    private void UploadGameInfoFromWechat(GameInfo gameInfo)
    {
        Debug.Log("调用上传用户数据");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "UploadGameInfo",
            data = gameInfo, // 用户数据类转为 JSON

            success = (res) =>
            {
                Debug.Log("上传用户数据成功");
            },
            fail = (res) =>
            {
                Debug.Log($"上传用户数据失败,+{res.errMsg}");
            },
            complete = (res) =>
            {
                Debug.Log("上传用户数据操作完成");
            }
        });
    }


    //退出到后台时自动上传数据
    private void AutoSaveGameInfoWeChat()
    {
        WX.OnHide((res) =>
        {
            string timeStr = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            UploadGameInfo(PlayerInfo);
        });
    }

    //同步皮肤数据到云端
    public void SyncDecalStatusFromDatabase()
    {
        if (SkinManager.instance.decalDatabase_meijia == null) return;

        foreach (var skin in PlayerInfo.meijiaDecals)
        {
            var matchingDecal = SkinManager.instance.decalDatabase_meijia.decalList.Find(d => d.id == skin.id);
            if (matchingDecal != null)
            {
                skin.isOwned = matchingDecal.isOwned;
                skin.isEquip = matchingDecal.isEquip;
            }
        }

        if (SkinManager.instance.decalDatabase_tiehua == null) return;

        foreach (var skin in PlayerInfo.tiehuaDecals)
        {
            var matchingDecal = SkinManager.instance.decalDatabase_tiehua.decalList.Find(d => d.id == skin.id);
            if (matchingDecal != null)
            {
                skin.isOwned = matchingDecal.isOwned;
                skin.isEquip = matchingDecal.isEquip;
            }
        }
    }
   

    //时间戳（秒）转日期字符串
    public string TimestampToDateString(long timestamp)
    {
        // 时间戳是以秒为单位
        DateTime dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    }

    //时间戳转为datetime
    public DateTime TimestampToDateTime(long timestamp)
    {
        // Unix 时间戳起点（1970-01-01 00:00:00 UTC）
        DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // 加上秒数
        return epoch.AddSeconds(timestamp);
    }


    // 判断两个时间戳是否为不同的一天（北京时间）
    public bool IsSameDay(long timestamp1, long timestamp2)
    {
        DateTime dt1 = DateTimeOffset.FromUnixTimeSeconds(timestamp1).ToLocalTime().Date;
        DateTime dt2 = DateTimeOffset.FromUnixTimeSeconds(timestamp2).ToLocalTime().Date;
        return dt1 == dt2;
    }
    //距离第二天 00:00:00还有多少秒
    public  int SecondsUntilNextDay(long timestamp)
    {
        DateTime current = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        DateTime nextDay = current.Date.AddDays(1); // 第二天的 00:00:00
        TimeSpan remaining = nextDay - current;
        return (int)remaining.TotalSeconds;
    }
}
