using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;



public class WechatManager : MonoBehaviour
{

    public static WechatManager instance;
    /// <summary>
    /// 屏幕宽度
    /// </summary>
    public static double ScreenWidth { get => WX.GetWindowInfo().screenWidth; }
    /// <summary>
    /// 屏幕高度
    /// </summary>
    public static double ScreenHeight { get => WX.GetWindowInfo().screenHeight; }

    public static double WindowWidth { get => WX.GetWindowInfo().windowWidth; }

    public static double WindowHeight { get => WX.GetWindowInfo().windowHeight; }

    public static double DPR { get => WX.GetWindowInfo().pixelRatio; }

    public RawImage RankBody;
    public Image imageLeftTop;
    public Image imageRightBottom;
    public GameObject RankObject;

    private GameInfo PlayerInfo=>GameEntrance.instance.PlayerInfo;

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
        UploadScore(PlayerInfo.level);
        RankObject.SetActive(true);
        ShowScore();
    }

    /// <summary>
    /// 关闭好友排行榜按钮
    /// </summary>
    public void CloseRankPanel()
    {
        AudioManager.instance.PlaySFX("点击");
        WX.HideOpenData();
        RankObject.SetActive(false);
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

public class MyOpendataMessage
{
    public string type;
    public int score;
}

[Serializable]
public class CloudResponse
{
     public GameInfo data; // 对应云函数返回的 "data" 字段
}