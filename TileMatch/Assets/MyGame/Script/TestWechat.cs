using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;
using WeChatWASM;

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
    private void OnEnable()
    {
        
    }

    private void Start()
    {
        scaler = RankObject.transform.parent.GetComponent<CanvasScaler>();
        RankObject.transform.position += new Vector3(10000, 0, 0);
        //ShowScore();
    }
    public  void UploadScore(int score)
    {
        MyOpendataMessage message = new MyOpendataMessage();
        message.type = "setUserRecord";
        message.score = score;
        string msg = JsonUtility.ToJson(message);
        WX.GetOpenDataContext().PostMessage(msg);
        Debug.Log("执行UploadScore(50)");
    }

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


        //Debug.Log($"width is {width},height is {height}");

        //WX.ShowOpenData(RankBody.texture, x, y,
        //    width,
        //    height);

        //Debug.Log($"referenceResoultion.x is {referenceResoultion.x},Screen.width is {Screen.width}");

        //Debug.Log($"RankBody.rectTransform.rect.width is {RankBody.rectTransform.rect.width},RankBody.rectTransform.rect.height {RankBody.rectTransform.rect.height}");
        //Debug.Log($"(int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.width) is {(int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.width)}," +
        //    $"(int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.height) {(int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.height)}");

        MyOpendataMessage msgData = new MyOpendataMessage();
        msgData.type = "showFriendsRank";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
        //Debug.Log("执行ShowScore");
    }

    public void RankButton()
    {
        RankObject.transform.position-=new Vector3(10000,0,0);
        LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
        //Debug.Log("最高通关："+levelSave.MaxReachedLevelIndex);
        Debug.Log(" RankObject.transform.position：" + RankObject.transform.position);
        UploadScore(levelSave.MaxReachedLevelIndex);
        StartCoroutine(DelayShowScore(0.1f));
    }

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

    // 在Scene视图中显示矩形
    //void OnDrawGizmos()
    //{
    //    CanvasScaler scaler = RankObject.GetComponent<CanvasScaler>();
    //    var referenceResoultion = scaler.referenceResolution;
    //    var p = RankBody.transform.position;
    //    // 设置Gizmos颜色为红色
    //    Gizmos.color = Color.red;

    //    //float h = (float)Screen.height * 1080 / Screen.width;
    //    //float delta = Screen.height - h;
    //    //float y = (h - (p.y - delta) /*- (buttonPosition.rect.height / 2)*/);
    //    //float tensileWidth = (((float)Screen.width / 1080) * RankBody.rectTransform.rect.width);
    //    //float tensileHeight = (((float)Screen.height / h) * RankBody.rectTransform.rect.height);


    //    //var width = (int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.width);
    //    //var height = (int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.height);
    //    //Debug.Log($"Screen.width is {Screen.width}, referenceResoultion.x is {referenceResoultion.x}");
    //    // 转换为矩形的屏幕坐标
    //    Rect rect = new Rect(imageLeftTop.transform.position.x, imageRightBottom.transform.position.y,
    //        GetWidth(),
    //        GetHeight());

    //    // 绘制矩形（在Scene视图中可见）
    //    Gizmos.DrawWireCube(new Vector3(rect.x + rect.width / 2, rect.y + rect.height / 2, 0), new Vector3(rect.width, rect.height, 0));
    //}

    int GetWidth()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        //// 将世界坐标转换为屏幕坐标
        //Vector3 screenPos1 = Camera.main.WorldToScreenPoint(worldPos1);
        //Vector3 screenPos2 = Camera.main.WorldToScreenPoint(worldPos2);

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);
        return (int)horizontalDistance;
    }

    int GetHeight()
    {
        // 获取 RectTransform
        RectTransform rect1 = imageLeftTop.GetComponent<RectTransform>();
        RectTransform rect2 = imageRightBottom.GetComponent<RectTransform>();

        // 获取两个 Image 的世界坐标
        Vector3 worldPos1 = rect1.position;
        Vector3 worldPos2 = rect2.position;

        //// 将世界坐标转换为屏幕坐标
        //Vector3 screenPos1 = Camera.main.WorldToScreenPoint(worldPos1);
        //Vector3 screenPos2 = Camera.main.WorldToScreenPoint(worldPos2);

        // 计算水平距离和竖直距离
        float horizontalDistance = Mathf.Abs(worldPos1.x - worldPos2.x);
        float verticalDistance = Mathf.Abs(worldPos1.y - worldPos2.y);
        return (int)verticalDistance;
    }
}
