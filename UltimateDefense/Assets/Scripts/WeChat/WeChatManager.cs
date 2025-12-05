using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeChatWASM;

public class WeChatManager : MonoBehaviour
{
    // 单例
    public static WeChatManager Instance;
    public static string UserCode=string.Empty;//微信用户的code，用于在Code2Session接口获取用户的UserID
    public static event UnityAction OnGetPlayerWechatInfo;//拿到玩家微信授权

    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //从云端加载数据
    public void InitSDK(UnityAction callback)
    {
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                callback?.Invoke();
            }

        );
    }



    public void CreateUserInfoButton()
    {
        WX.GetSetting(new GetSettingOption()
        {
            success = (res) =>
            {
                Debug.Log($"获取用户权限: {JsonConvert.SerializeObject(res.authSetting, Formatting.Indented)}");
                //已经授权过
                if (res.authSetting.ContainsKey("scope.userInfo") && res.authSetting["scope.userInfo"] == true)
                {

                }
                else
                {
                    Debug.Log($"还未获取过权限");
                }
                //无论有没有获取到玩家权限，都要进行登录
                
            },

            fail = (res) =>
            {
                Debug.Log($"获取Setting失败：{res.errMsg}");
            }
        });

    }


    //短震动
    public void Vibrate(string type="light")
    {
        WX.VibrateShort(new VibrateShortOption()
        {
            type = type,
            complete = (res) =>
            {
                Debug.Log("震动结束："+res.errMsg);
            }
        });
    }

    public static void ShareApp(UnityAction callback, string title = null, string imageUrl = null, string imageUrlId = null)
    {
        // 主动拉起分享给通讯录的方法
        WX.ShareAppMessage(new ShareAppMessageOption()
        {
            title = title,
            imageUrl = imageUrl,
            imageUrlId = imageUrlId
        });
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


    

}

public class MyOpendataMessage
{
    public string type;
    public int score;
}
