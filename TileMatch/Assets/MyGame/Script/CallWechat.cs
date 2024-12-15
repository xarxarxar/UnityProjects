using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using WeChatWASM;

public class CallWechat : MonoBehaviour
{
    public static Font wxFont;

    public static void Init() 
    {
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
            }
        );

        var feedback = "https://7465-test01cloud-8g9b0glp7aab2737-1322886618.tcb.qcloud.la/mainFolder/Fonts/Source_Han_Sans_SC_Normal_Normal.otf?sign=4665881d91f1c1b11adbb160cad2ced5&t=1729609199";
        WX.GetWXFont(feedback, (font) =>
        {
            wxFont = font;
            //更换微信字体
            TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            // 创建字体资产
            TMP_FontAsset wxFontAsset = TMP_FontAsset.CreateFontAsset(font);

            for (int i = 0; i < allTexts.Length; i++)
            {
                allTexts[i].font = wxFontAsset;
            }
        });

    }

    public  static void ShareApp(UnityAction callback)
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
    /// 上传用户数据
    /// </summary>
    public void UploadUserData()
    {

    }

    /// <summary>
    /// 下载用户数据
    /// </summary>
    public void DownloadUserData()
    {

    }

    /// <summary>
    /// 创建用户数据
    /// </summary>
    private void CreateUserData()
    {
        
    }

}
