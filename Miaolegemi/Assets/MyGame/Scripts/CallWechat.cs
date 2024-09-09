using System;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using UnityEngine.UI;

public class CallWechat : MonoBehaviour
{
    /// <summary>
    /// 单例模式实例
    /// </summary>
    public static CallWechat instance;

    /// <summary>
    /// 当前用户数据
    /// </summary>
    public UserData thisUserData = new UserData();

    /// <summary>
    /// 从数据库中获取的所有卡牌数据
    /// </summary>
    public List<ItemContent> allItems = new List<ItemContent>();

    private void Awake()
    {
        instance = this; // 初始化单例
    }

    private void Start()
    {
        // 初始化用户数据
        thisUserData = new UserData
        {
            UserNickName = "def",
            AvatarURL = "123",
            UserScore = 20,
            cardData = new List<CardData>()
        };

        for (int i = 0; i < allItems.Count; i++)
        {
            thisUserData.cardData.Add(new CardData(allItems[i].Name, 0, allItems[i].Weight, 0));
        }

        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                // 初始化云环境
                WX.cloud.Init(new CallFunctionInitParam()
                {
                    env = "test01cloud-8g9b0glp7aab2737", // 云环境 ID
                    traceUser = false
                });
                
                // 获取用户信息（授权）
                GetUserInfo();

                // 显示分享菜单
                ShowShareMenuOption ssmo = new ShowShareMenuOption();
                ssmo.menus = new string[] { "shareAppMessage", "shareTimeline" };
                //WX.ShowShareMenu(ssmo); 

                // 主动拉起分享给通讯录的方法
                WX.ShareAppMessage(new ShareAppMessageOption());

                // 当小游戏从后台转回前台时调用的方法
                WX.OnShow((res) =>
                {
                    Debug.Log("游戏从后台回到前台");
                });
            }
        );
    }

    /// <summary>
    /// 在数据库中创建用户数据，如果已经存在则不创建
    /// </summary>
    /// <param name="gameUserData">游戏用户数据</param>
    public void CreateUserData(UserData gameUserData)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "create_userdata",
            data = JsonUtility.ToJson(gameUserData), // 用户数据类转为 JSON

            success = (res) =>
            {
                Debug.Log("创建用户数据成功");
                GetCardData((res) => { }); // 从数据库获取用户信息，包括游戏信息
            },
            fail = (res) =>
            {
                Debug.Log("创建用户数据失败");
            },
            complete = (res) =>
            {
                Debug.Log("创建用户数据操作完成");
            }
        });
    }

    /// <summary>
    /// 上传用户数据到云端
    /// </summary>
    /// <param name="gameUserData">游戏用户数据</param>
    public void CallSetUserData(UserData gameUserData)
    {
        Debug.Log("调用上传用户数据");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "upload-userdata",
            data = JsonUtility.ToJson(gameUserData), // 用户数据类转为 JSON

            success = (res) =>
            {
                Debug.Log("上传用户数据成功");
            },
            fail = (res) =>
            {
                Debug.Log("上传用户数据失败");
            },
            complete = (res) =>
            {
                Debug.Log("上传用户数据操作完成");
            }
        });
    }

    /// <summary>
    /// 从云数据库获取卡牌数据
    /// </summary>
    /// <param name="successAction">获取成功后的回调函数</param>
    public void GetCardData(Action<OnlineUserdata> successAction)
    {
        LoadingManager.Instance.StartLoading();

        OnlineUserdata userData = null; // 用于存储用户数据的变量

        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",
            data = "{\"player_data\":0}", // 下载时需要随便传一个 JSON，否则会报错

            success = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("获取卡牌数据成功");

                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());
                    Debug.Log($"用户数据为：{userData.data.cardData}");
                    thisUserData.UserScore = userData.data.UserScore;
                    thisUserData.cardData = userData.data.cardData;

                    for (int i = 0; i < thisUserData.cardData.Count; i++)
                    {
                        Debug.Log($"卡牌数量：{thisUserData.cardData[i].Count}");
                    }
                    successAction?.Invoke(userData);
                }
            },
            fail = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.LogError("获取卡牌数据失败：" + res.errMsg);
            },
            complete = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("获取卡牌数据操作完成");
            }
        });
    }

    /// <summary>
    /// 获取排行榜信息
    /// </summary>
    /// <param name="successAction">获取成功后的回调函数</param>
    public void GetRankInfo(Action<RankInfo> successAction)
    {
        LoadingManager.Instance.StartLoading();
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-rankinfo",
            data = "{\"player_data\":0}", // 下载时需要随便传一个 JSON，否则会报错

            success = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                if (res.result != null)
                {
                    Debug.Log($"排行榜数据：{res.result}");
                    RankInfo rankInfo = JsonUtility.FromJson<RankInfo>(res.result.ToString());
                    Debug.Log($"用户昵称：{rankInfo.data[0].gamedata.UserNickName}");
                    successAction?.Invoke(rankInfo);
                }
            },
            fail = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.LogError("获取排行榜信息失败：" + res.errMsg);
            },
            complete = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("获取排行榜信息操作完成");
            }
        });
    }

    /// <summary>
    /// 获取用户的微信ID和头像（未授权时调用）
    /// </summary>
    public void GetUserWechatIDandAvatar()
    {
        // 创建授权按钮
        WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", false);
        btn.Show();
        btn.OnTap((res) =>
        {
            if (res.errCode == 0)
            {
                // 用户已允许获取个人信息
                Debug.Log("用户信息：" + JsonUtility.ToJson(res.userInfo, true));
                WXUserInfo userInfo = res.userInfo;
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;
            }
            else
            {
                Debug.Log("用户拒绝获取个人信息");
            }
            btn.Hide();
            Debug.Log("已隐藏授权区域");
        });
    }

    /// <summary>
    /// 调用API获取用户信息（已授权后调用）
    /// </summary>
    public void GetUserInfo()
    {
        WX.GetUserInfo(new GetUserInfoOption()
        {
            lang = "zh_CN",
            success = (res) =>
            {
                Debug.Log("获取用户信息成功（API）：" + JsonUtility.ToJson(res.userInfo, true));
                WXUserInfo userInfo = ConvertUserInfo(res.userInfo);
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                CreateUserData(thisUserData);
            },
            fail = (res) =>
            {
                Debug.Log("获取用户信息失败");
            }
        });
    }

    /// <summary>
    /// 将JSON解析的用户信息转为C#类对象
    /// </summary>
    /// <param name="jsonObject">JSON格式的用户信息</param>
    /// <returns>解析后的用户信息类对象</returns>
    private WXUserInfo ConvertUserInfo(object jsonObject)
    {
        return JsonUtility.FromJson<WXUserInfo>(JsonUtility.ToJson(jsonObject));
    }
}
