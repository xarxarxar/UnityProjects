using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一些暂时没用，但是可能有用的备用代码
/// </summary>
public class Standby : MonoBehaviour
{
    //WX.cloud.CallFunction(new CallFunctionParam()
    //{
    //    name = "download-file",  // 替换为你获取玩家数据的云函数名称
    //    data = "{\"player_data\":0}",  //下载的时候 这里的 data 需要随便传一个 json，否则会报错,  // 你可以根据需要传递参数

    //    success = (res) =>
    //    {
    //        Debug.Log("GetCardData success");
    //        WX.ShowModal(new ShowModalOption
    //        {
    //            title = "标题",
    //            content = res.ToString(),
    //            success = (res) =>
    //            {
    //                if (res.confirm)
    //                {
    //                    Debug.Log("点击了确定按钮");
    //                    WX.ShowToast(new ShowToastOption
    //                    {
    //                        title = "消息提示框"
    //                    });
    //                }
    //                else if (res.cancel)
    //                {
    //                    Debug.Log("点击了取消按钮");
    //                }
    //            }
    //        });
    //    },
    //    fail = (res) =>
    //    {
    //        Debug.LogError("GetCardData failed: " + res.errMsg);
    //    },
    //    complete = (res) =>
    //    {
    //        Debug.Log("GetCardData complete");
    //    }
    //});

    //Debug.Log(code);


    //GetCardData((userdata) =>
    //{
    //    WX.ShowModal(new ShowModalOption
    //    {
    //        title = "标题",
    //        content = userdata.UserName,
    //        success = (res) =>
    //        {
    //            if (res.confirm)
    //            {
    //                Debug.Log("点击了确定按钮");
    //                WX.ShowToast(new ShowToastOption
    //                {
    //                    title = "消息提示框"
    //                });
    //            }
    //            else if (res.cancel)
    //            {
    //                Debug.Log("点击了取消按钮");
    //            }
    //        }
    //    });
    //});
}
