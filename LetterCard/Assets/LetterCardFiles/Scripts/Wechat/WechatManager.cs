using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private void Start()
    {
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                 WX.cloud.Init(new ICloudConfig()
                {
                    env = "cloud1-1g93cld7637aacb4", // 云环境 ID
                    traceUser = false
                });
            }
        );
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
            //data = JsonUtility.ToJson(playerInfo), // 用户数据类转为 JSON
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


    /// <summary>
    /// 从云数据库获取卡牌数据
    /// </summary>
    /// <param name="successAction">获取成功后的回调函数</param>
    public static void GetUserData(UnityAction successAction)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",
            data = "{\"player_data\":0}", // 下载时需要随便传一个 JSON，否则会报错

            success = (res) =>
            {
                Debug.Log("获取卡牌数据成功");
                
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
        UploadScore(10);
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

}
