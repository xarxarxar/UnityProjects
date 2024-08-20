using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class CallWechat : MonoBehaviour
{
    public static CallWechat instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UserData thisUserData = new UserData
        {
            UserName = "def",
            UserID = "456",
            UserScore = 20,
            testName = 50,
            cardData = new List<CardData>
            {
                new CardData("def", "猫咪碎片1", 30, 3, 0),
                new CardData("def", "猫咪碎片2", 30, 3, 0),
                new CardData("def", "猫咪碎片3", 30, 3, 0),
                new CardData("def", "猫咪碎片4", 30, 3, 0),
                new CardData("def", "猫咪碎片5", 30, 3, 0),
                new CardData("def", "猫咪碎片6", 30, 3, 0),
                new CardData("def", "猫咪碎片7", 30, 3, 0)
            }
        };

        WX. InitSDK(
            (code) =>
            {
                WX.cloud.Init(new CallFunctionInitParam()
                {
                    env = "test01cloud-8g9b0glp7aab2737",
                    traceUser = false
                });
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

                ShowShareMenuOption ssmo = new ShowShareMenuOption();
                ssmo.menus = new string[] { "shareAppMessage", "shareTimeline" };
                //WX.ShowShareMenu(ssmo); // 这个是控制右上角三个点是否可以点击的方法，并不是主动分享给好友的方法！！！

                // 这个才是主动拉起分享给通讯录的方法！！！
                WX.ShareAppMessage(new ShareAppMessageOption());

                //检测小游戏从台后转到台前调用的方法，可以在分享结束之后调用该方法
                WX.OnShow((res) =>
                {
                    Debug.Log("i  m back");
                });

                CallSetUserData(thisUserData);
                
            }
        );

           }


    private void CallSetUserData(UserData gameUserData)
    {
        Debug.Log("CallSetUserData");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "TestCloudFunction01",
            //用户数据类转为json，测试时我试过传空串会导致云函数报错，原因不明，懂得大佬可以评论区教教我哈哈
            data = JsonUtility.ToJson(gameUserData),

            success = (res) =>
            {
                Debug.Log("CallSetUserData success");
                //Debug.Log(res.result);
            },
            fail = (res) =>
            {
                Debug.Log("fail");
                //Debug.Log(res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("complete");
                //Debug.Log(res.result);
            }
        });
    }

    public void GetCardData(Action<OnlineUserdata> successAction)
    {
        // 创建一个用于存储用户数据的变量
        OnlineUserdata userData = null;

        // 调用云函数 "TestCloudFunction02" (假设这是你之前定义的获取玩家数据的云函数名称)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",  // 替换为你获取玩家数据的云函数名称
            data = "{\"player_data\":0}",  //下载的时候 这里的 data 需要随便传一个 json，否则会报错,  // 你可以根据需要传递参数

            success = (res) =>
            {
                Debug.Log("GetCardData success");
                
                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    Debug.Log(res.result);
                    // 将 JSON 转换为 UserData 对象
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());
                    Debug.Log(userData.data.UserName);
                    // 调用传入的成功回调函数，传递 userData
                    successAction?.Invoke(userData);
                }
            },
            fail = (res) =>
            {
                Debug.LogError("GetCardData failed: " + res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("GetCardData complete");
            }
        });
    }
}
