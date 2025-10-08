using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;

public class WeChatManager : ManagerBase<WeChatManager>
{
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.OutBattle;//局外Manager
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        //LoadDataFromCloud();
    }

    //从云端加载数据
    private void LoadDataFromCloud()
    {
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                
            }
        );

    }

    //短震动
    public void Vibrate()
    {
        WX.VibrateShort(new VibrateShortOption()
        {
            type = "heavy",
            complete = (res) =>
            {
                Debug.Log("震动结束："+res.errMsg);
            }
        });
    }
}
