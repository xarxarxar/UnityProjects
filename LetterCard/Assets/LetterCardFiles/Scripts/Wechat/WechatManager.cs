using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;

public class MyOpendataMessage
{
    public string type;
    public int score;
}

public class WechatManager : MonoBehaviour
{

    public static WechatManager instance;
    /// <summary>
    /// 屏幕宽度
    /// </summary>
    public static double ScreenWidth { get => WX.GetWindowInfo().screenWidth;}
    /// <summary>
    /// 屏幕高度
    /// </summary>
    public static double ScreenHeight {  get => WX.GetWindowInfo().screenHeight;}
    
    public static double WindowWidth {  get => WX.GetWindowInfo().windowWidth;}

    public static double WindowHeight {  get => WX.GetWindowInfo().windowHeight;}

    public static double DPR { get=>WX.GetWindowInfo().pixelRatio;}

    public RawImage RankBody;
    public Image imageLeftTop;
    public Image imageRightBottom;
    public GameObject RankObject;
    WXUserInfoButton wxUserInfoButton;

    private void Awake()
    {
        instance = this;
    }

    public static void ShareApp(UnityAction callback)
    {
        // 主动拉起分享给通讯录的方法
        WX.ShareAppMessage(new ShareAppMessageOption());
        System.Action<OnShowListenerResult> res = null;
        res = (result) =>
        {
            // 先执行传入的回调
            callback();

            // 再取消 OnShow 监听
            WX.OffShow(res);
        };

        // 设置 OnShow 监听
        WX.OnShow(res);
    }


    /// <summary>
    /// 上传用户数据到云端
    /// </summary>
    /// <param name="playerInfo">游戏用户数据</param>
    public static void CallSetUserData(PlayerInfo playerInfo)
    {
        Debug.Log("调用上传用户数据");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "UploadPlayerInfo",
            data = playerInfo, // 用户数据类转为 JSON

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


    public void CreateUserInfoButton()
    {
        WX.GetSetting(new GetSettingOption()
        {
            success = (res) =>
            {
                Debug.Log($"获取Setting成功");
                //已经授权过
                if (res.authSetting["scope.userInfo"] == true)
                {
                    wxUserInfoButton.Hide();
                    Debug.Log($"已经获取过权限");
                    WX.GetUserInfo(new GetUserInfoOption()
                    {
                        success = (res) =>
                        {
                            Debug.Log($"获取用户信息成功:{res.userInfo.nickName}");
                            ShowTipManager.instance.ShowLoading(true);
                            DataManager.instance.DownloadPlayerInfo(() =>
                            {
                                if (res.userInfo.nickName != DataManager.instance.globalPlayerInfo.playerName
                            || res.userInfo.avatarUrl != DataManager.instance.globalPlayerInfo.avatarUrl)
                                {
                                    DataManager.instance.globalPlayerInfo.playerName = res.userInfo.nickName;
                                    DataManager.instance.globalPlayerInfo.avatarUrl = res.userInfo.avatarUrl;
                                }
                                DataManager.instance.UploadPlayerInfo();
                                ShowTipManager.instance.ShowLoading(false);
                                GameEntrance.instance.CloseEnterGamePanel();
                            });

                            //ShowTipManager.instance.ShowLoading(false);
                        },
                        fail = (res) =>
                        {
                            Debug.LogError("获取用户信息失败：" + res.errMsg);
                            ShowTipManager.instance.ShowTip("获取用户信息失败");
                            //ShowTipManager.instance.ShowLoading(false);
                        },
                        complete = (res) =>
                        {
                            Debug.Log("获取用户信息操作完成");
                        }
                    }); 
                }
                else
                {
                    Debug.Log($"还未获取过权限");
                    wxUserInfoButton.Show();
                }
            },

            fail = (res) =>
            {
                Debug.Log($"获取Setting失败：{res.errMsg}");
            }
        });
        
    }

    public void CreateUserInfoButtonBefore()
    {
        Rect rect = GetStartButtonRect();
        wxUserInfoButton = WX.CreateUserInfoButton((int)rect.x, Screen.height - (int)rect.y - (int)rect.height, (int)rect.width, (int)rect.height, "", true);
        wxUserInfoButton.OnTap((res) =>
        {
            if (res.errCode == 0)
            {
                wxUserInfoButton.Hide();
                WX.GetUserInfo(new GetUserInfoOption()
                {
                    success = (res) =>
                    {
                        Debug.Log($"获取用户信息成功:{res.userInfo.nickName}");
                        ShowTipManager.instance.ShowLoading(true);
                        DataManager.instance.DownloadPlayerInfo(() =>
                        {
                            if (res.userInfo.nickName != DataManager.instance.globalPlayerInfo.playerName
                        || res.userInfo.avatarUrl != DataManager.instance.globalPlayerInfo.avatarUrl)
                            {
                                DataManager.instance.globalPlayerInfo.playerName = res.userInfo.nickName;
                                DataManager.instance.globalPlayerInfo.avatarUrl = res.userInfo.avatarUrl;
                            }
                            DataManager.instance.UploadPlayerInfo();
                            ShowTipManager.instance.ShowLoading(false);
                            GameEntrance.instance.CloseEnterGamePanel();
                        });
                    },
                    fail = (res) =>
                    {
                        Debug.LogError("获取用户信息失败：" + res.errMsg);
                        ShowTipManager.instance.ShowTip("获取用户信息失败");
                    },
                    complete = (res) =>
                    {
                        Debug.Log("获取用户信息操作完成");
                    }
                });
            }
            else
            {
                ShowTipManager.instance.ShowTip("请先授权");
            }
        });
        wxUserInfoButton.Hide();

    }


    /// <summary>
    /// 从云数据库获取卡牌数据
    /// </summary>
    /// <param name="successAction">获取成功后的回调函数</param>
    public static void GetUserData(UnityAction<PlayerInfo> successAction)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "DownloadPlayerInfo",
            //data = "{\"player_data\":0}", // 下载时需要随便传一个 JSON，否则会报错

            success = (res) =>
            {
                Debug.Log($"获取卡牌数据成功:{res.result}");
                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    CloudResponse response = new CloudResponse();
                    response = JsonUtility.FromJson<CloudResponse>(res.result);
                    // 再提取实际数据
                    PlayerInfo localUserData = new PlayerInfo();
                    localUserData = response.data;
                    Debug.Log($"用户coinCount为：{localUserData.coinCount}");
                    Debug.Log($"用户maxRound为：{localUserData.maxRound}");
                    Debug.Log($"用户maxScore为：{localUserData.maxScore}");
                    successAction?.Invoke(localUserData);
                }
            },
            fail = (res) =>
            {
                Debug.LogError("获取卡牌数据失败：" + res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("获取卡牌数据操作完成");
            }
        });
    }


    /// <summary>
    /// 上传分数
    /// </summary>
    /// <param name="score"></param>
    public static void UploadScore(int score)
    {
        MyOpendataMessage message = new MyOpendataMessage();
        message.type = "setUserRecord";
        message.score = score;
        string msg = JsonUtility.ToJson(message);
        WX.GetOpenDataContext().PostMessage(msg);
    }

    /// <summary>
    /// 获取微信端的分数数据，并显示
    /// </summary>
    public void ShowScore()
    {
        var p = RankBody.transform.position;
        float h = (float)Screen.height * 1080 / Screen.width;
        float delta = Screen.height - h;
        float y = (h - (p.y - delta) /*- (buttonPosition.rect.height / 2)*/);
        WX.ShowOpenData(RankBody.texture, (int)imageLeftTop.transform.position.x, Screen.height - (int)imageLeftTop.transform.position.y,
        GetWidth(), GetHeight()); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点
        MyOpendataMessage msgData = new MyOpendataMessage();
        msgData.type = "showFriendsRank";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
    }

    /// <summary>
    /// 好友排行榜按钮
    /// </summary>
    public void RankButton()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        UploadScore(DataManager.instance.globalPlayerInfo.maxRound);
        RankObject.SetActive(true);
        ShowScore();
        //RankObject.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        //{
            
        //});
    }

    /// <summary>
    /// 关闭好友排行榜按钮
    /// </summary>
    public void CloseRankPanel()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        WX.HideOpenData();
        RankObject.SetActive(false);
        //RankObject.transform.DOScale(0, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        //{
            
        //});
        //RankObject.transform.position += new Vector3(10000, 0, 0);
    }



    //获取排行榜显示区域的宽
    int GetWidth()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);

        Debug.Log($"宽度为{(int)horizontalDistance}");
        return (int)horizontalDistance;
    }
    //获取排行榜显示区域的高
    int GetHeight()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);

        Debug.Log($"长度为{(int)verticalDistance}");
        return (int)verticalDistance;
    }


    Rect GetStartButtonRect()
    {
        var rectTransform = GameEntrance.instance.startGameButton.GetComponent<RectTransform>();
        // 获取 RectTransform 的四个角的世界坐标
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 创建屏幕矩形
        var screenRect = new Rect(
                        worldCorners[0].x,
                        worldCorners[0].y,
                        worldCorners[2].x - worldCorners[0].x,
                        worldCorners[2].y - worldCorners[0].y);

        Debug.Log($"Screen Rect: {screenRect}");
        return screenRect;
    }
}
