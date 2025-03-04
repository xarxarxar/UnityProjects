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

    private void Start()
    {
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {

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

    /*
     * CallWechat.ShareApp(() =>
            {
                // 更新保存的计时器开始时间
                save.Value = DateTime.Now.ToBinary();
                timerStartTime = DateTime.Now;
                PUController.AddPowerUp(PUType.Undo, 1);//添加一个“提示”道具的事例
                AudioController.PlaySound(AudioController.Sounds.buySuccess);
            });
     */


    /// <summary>
    /// 上传用户数据到云端
    /// </summary>
    /// <param name="gameUserData">游戏用户数据</param>
    //public static void CallSetUserData(LocalUserData gameUserData)
    //{
    //    Debug.Log("调用上传用户数据");
    //    WX.cloud.CallFunction(new CallFunctionParam()
    //    {
    //        name = "upload-userdata",
    //        data = JsonUtility.ToJson(gameUserData), // 用户数据类转为 JSON

    //        success = (res) =>
    //        {
    //            Debug.Log("上传用户数据成功");
    //        },
    //        fail = (res) =>
    //        {
    //            Debug.Log("上传用户数据失败");
    //        },
    //        complete = (res) =>
    //        {
    //            Debug.Log("上传用户数据操作完成");
    //        }
    //    });
    //}

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
        //MyOpendataMessage message = new MyOpendataMessage();
        //message.type = "setUserRecord";
        //message.score = score;
        //string msg = JsonUtility.ToJson(message);
        //WX.GetOpenDataContext().PostMessage(msg);
        Debug.Log("执行UploadScore(50)");
    }

    /// <summary>
    /// 获取微信端的分数数据，并显示
    /// </summary>
    public void ShowScore()
    {
        //CanvasScaler scaler = RankObject.GetComponent<CanvasScaler>();
        //var referenceResoultion = scaler.referenceResolution;
        //var p = RankBody.transform.position;

        //var x = (int)(p.x - 0.5f * RankBody.rectTransform.rect.width);
        //var y = (int)(referenceResoultion.y-(int)p.y - 0.5f * RankBody.rectTransform.rect.height);
        //var width = (int)RankBody.rectTransform.rect.width;
        //var height = (int)RankBody.rectTransform.rect.height;


        float h = (float)Screen.height * 1080 / Screen.width;
        float delta = Screen.height - h;
        //float y = (h - (p.y - delta) /*- (buttonPosition.rect.height / 2)*/);
        //float tensileWidth = (((float)Screen.width / 1080) * RankBody.rectTransform.rect.width);
        //float tensileHeight = (((float)Screen.height / h) * RankBody.rectTransform.rect.height);
        //WX.ShowOpenData(RankBody.texture, (int)p.x, (int)y, (int)tensileWidth, (int)tensileHeight); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点
        //WX.ShowOpenData(RankBody.texture, (int)imageLeftTop.transform.position.x, Screen.height - (int)imageLeftTop.transform.position.y,
        //GetWidth(), GetHeight()); //高 值变小 拉伸    小拉伸 所以 宽 大点  或者 高小点

        //WX.ShowOpenData(RankBody.texture, x, y,
        //    width,
        //    height);

        //MyOpendataMessage msgData = new MyOpendataMessage();
        //msgData.type = "showFriendsRank";
        //string msg = JsonUtility.ToJson(msgData);
        //WX.GetOpenDataContext().PostMessage(msg);
    }

    /// <summary>
    /// 好友排行榜按钮
    /// </summary>
    public void RankButton()
    {
        WX.HideOpenData();
        //RankObject.transform.position -= new Vector3(10000, 0, 0);
        //LevelSave levelSave = SaveController.GetSaveObject<LevelSave>("level");
        //Debug.Log("最高通关："+levelSave.MaxReachedLevelIndex);
        //Debug.Log(" RankObject.transform.position：" + RankObject.transform.position);
        //UploadScore(levelSave.MaxReachedLevelIndex);
    }

    /// <summary>
    /// 关闭好友排行榜按钮
    /// </summary>
    public void CloseRankPanel()
    {
        WX.HideOpenData();
        //RankObject.SetActive(false);
        //RankObject.transform.position += new Vector3(10000, 0, 0);
    }

    

    
    
}
