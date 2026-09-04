using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using WeChatWASM;
using UnityEngine.Events;

public class MyOpendataMessage
{
    public string type;
    public int score;
}



public class TestWechat : MonoBehaviour
{
    public RawImage RankBody;
    public GameObject RankObject;
    public Image imageLeftTop;
    public Image imageRightBottom;
    CanvasScaler scaler;

    private const string USER_DATA_KEY = "LOCAL_USER_DATA";
    private void OnEnable()
    {
       
    }

    private void Start()
    {
        scaler = RankObject.transform.parent.GetComponent<CanvasScaler>();
        RankObject.transform.position += new Vector3(10000, 0, 0);
        //ShowScore();
    }

    /// <summary>
    /// 上传用户数据到云端
    /// </summary>
    /// <param name="gameUserData">游戏用户数据</param>
    public static void CallSetUserData(LocalUserData gameUserData)
    {
        if (gameUserData == null)
        {
            Debug.LogError("保存数据失败：数据为空");
            return;
        }

        string json = JsonUtility.ToJson(gameUserData);

        PlayerPrefs.SetString(USER_DATA_KEY, json);
        PlayerPrefs.Save();

        Debug.Log("本地用户数据保存成功：" + json);
    }

    /// <summary>
    /// 从云数据库获取卡牌数据
    /// </summary>
    /// <param name="successAction">获取成功后的回调函数</param>
    public static void  GetUserData(UnityAction<LocalUserData> successAction)
    {
        if (!PlayerPrefs.HasKey(USER_DATA_KEY))
        {
            Debug.LogWarning("本地没有用户数据，创建默认数据");

            LocalUserData defaultData = CreateDefaultUserData();
            CallSetUserData(defaultData);

            successAction?.Invoke(defaultData);
            return;
        }

        string json = PlayerPrefs.GetString(USER_DATA_KEY);

        LocalUserData data = JsonUtility.FromJson<LocalUserData>(json);

        Debug.Log("读取本地用户数据成功：" + json);

        successAction?.Invoke(data);
    }


    private static LocalUserData CreateDefaultUserData()
    {
        return new LocalUserData()
        {
            IsActived = 0,
            CoinCount = 0,
            MaxLifeCount = 5,
            CurrentLiftCount = 5,
            LifeDuration = 300, // 5分钟恢复一个

            MaxLevelCount = 1,

            FreeCoinButtonLastTime = 0,
            ShareLifeButtonLastTime = 0,
            SharePromptButtonLastTime = 0,
            ShareShuffleButtonLastTime = 0,
            ShareUndoButtonLastTime = 0,
            ShareAddButtonLastTime = 0
        };
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
        Debug.Log("执行UploadScore(50)");
    }

    /// <summary>
    /// 获取微信端的分数数据，并显示
    /// </summary>
    public  void ShowScore()
    {
        //CanvasScaler scaler = RankObject.GetComponent<CanvasScaler>();
        var referenceResoultion = scaler.referenceResolution;
        var p = RankBody.transform.position;

        //var x = (int)(p.x - 0.5f * RankBody.rectTransform.rect.width);
        //var y = (int)(referenceResoultion.y-(int)p.y - 0.5f * RankBody.rectTransform.rect.height);
        //var width = (int)RankBody.rectTransform.rect.width;
        //var height = (int)RankBody.rectTransform.rect.height;


        float h = (float)Screen.height * 1080 / Screen.width;
        float delta = Screen.height - h;
        float y = (h - (p.y - delta) /*- (buttonPosition.rect.height / 2)*/);
        float tensileWidth = (((float)Screen.width / 1080) * RankBody.rectTransform.rect.width);
        float tensileHeight = (((float)Screen.height / h) * RankBody.rectTransform.rect.height);
        //WX.ShowOpenData(RankBody.texture, (int)p.x, (int)y, (int)tensileWidth, (int)tensileHeight); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点
        WX.ShowOpenData(RankBody.texture, (int)imageLeftTop.transform.position.x, Screen.height -(int)imageLeftTop.transform.position.y,
        GetWidth(), GetHeight()); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点

        //WX.ShowOpenData(RankBody.texture, x, y,
        //    width,
        //    height);

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
        WX.HideOpenData();
        RankObject.transform.position-=new Vector3(10000,0,0);
        LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
        //Debug.Log("最高通关："+levelSave.MaxReachedLevelIndex);
        Debug.Log(" RankObject.transform.position：" + RankObject.transform.position);
        UploadScore(levelSave.MaxReachedLevelIndex);
        StartCoroutine(DelayShowScore(0.1f));
    }

    /// <summary>
    /// 关闭好友排行榜按钮
    /// </summary>
    public void CloseRankPanel()
    {
        WX.HideOpenData();
        //RankObject.SetActive(false);
        RankObject.transform.position += new Vector3(10000, 0, 0);
    }

    IEnumerator DelayShowScore(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowScore();
        yield break;
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
        return (int)verticalDistance;
    }
}
