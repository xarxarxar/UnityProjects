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

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        
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
        CanvasScaler scaler = RankObject.GetComponent<CanvasScaler>();
        var referenceResoultion = scaler.referenceResolution;
        var p = RankBody.transform.position;
        WX.ShowOpenData(RankBody.texture, (int)p.x, Screen.height - (int)p.y,
            (int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.width),
            (int)(Screen.width / referenceResoultion.x * RankBody.rectTransform.rect.height));

        MyOpendataMessage msgData = new MyOpendataMessage();
        msgData.type = "showFriendsRank";
        string msg = JsonUtility.ToJson(msgData);
        WX.GetOpenDataContext().PostMessage(msg);
        Debug.Log("执行ShowScore");
    }

    public void RankButton()
    {
        RankObject.SetActive(true);
        LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
        Debug.Log("最高通关："+levelSave.MaxReachedLevelIndex);
        UploadScore(levelSave.MaxReachedLevelIndex);
        ShowScore();
    }

    public void CloseRankPanel()
    {
        RankObject.SetActive(false);
    }
}
