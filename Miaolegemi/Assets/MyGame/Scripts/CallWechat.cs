using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.UI;

public class CallWechat : MonoBehaviour
{
    public static CallWechat instance;

    public UserData thisUserData = new UserData();

    public Text AvatarText; 
    public Image AvatarImage;//头像图片
    public Text NickNameText;//昵称文本组件     

    //从数据库中获取的所有卡牌，暂时用list代替
    public List<ItemContent> allItems = new List<ItemContent>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        thisUserData = new UserData
        {
            UserNickName = "def",
            AvatarURL = "123",
            UserScore = 20,
            cardData = new List<CardData>
            {
                new CardData("1", 0, 3, 0),
                new CardData("2", 0, 3, 0),
                new CardData("3", 0, 3, 0),
                new CardData("4", 0, 2, 0),
                new CardData("5", 0, 2, 0),
                new CardData("6", 0, 2, 0),
                new CardData("7", 0, 2, 0),
                new CardData("8", 0, 1, 0),
                new CardData("9", 0, 1, 0),
                new CardData("10", 0, 1, 0),
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
                GetUserInfo();//用户授权获取昵称和头像信息
                
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

                //CallSetUserData(thisUserData);
                
            }
        );

           }

    //在数据库中创建userdata，如果已有的话就不创建
    public void CreateUserData(UserData gameUserData)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "create_userdata",
            //用户数据类转为json，测试时我试过传空串会导致云函数报错，原因不明，懂得大佬可以评论区教教我哈哈
            data = JsonUtility.ToJson(gameUserData),

            success = (res) =>
            {
                Debug.Log("create UserData success");
                GetCardData((res) => { });//从数据库获取用户信息，包括游戏信息
            },
            fail = (res) =>
            {
                Debug.Log("fail");
            },
            complete = (res) =>
            {
                Debug.Log("complete");
            }
        });
    }

    public void CallSetUserData(UserData gameUserData)
    {
        Debug.Log("CallSetUserData");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "upload-userdata",
            //name = "TestCloudFunction01",
            //用户数据类转为json，测试时我试过传空串会导致云函数报错，原因不明，懂得大佬可以评论区教教我哈哈
            data = JsonUtility.ToJson(gameUserData),

            success = (res) =>
            {
                Debug.Log("CallSetUserData success");
            },
            fail = (res) =>
            {
                Debug.Log("fail");
            },
            complete = (res) =>
            {
                Debug.Log("complete");
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
                    // 将 JSON 转换为 UserData 对象
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());

                    Debug.Log($"userData is {userData.data.cardData}");
                    thisUserData.UserScore = userData.data.UserScore;
                    thisUserData.cardData = userData.data.cardData;

                    for (int i = 0; i < thisUserData.cardData.Count; i++)
                    {
                        Debug.Log(thisUserData.cardData[i].Count);
                    }
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

    public void GetRankInfo(Action<RankInfo> successAction)
    {

        // 调用云函数 "TestCloudFunction02" (假设这是你之前定义的获取玩家数据的云函数名称)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-rankinfo",  // 替换为你获取玩家数据的云函数名称
            data = "{\"player_data\":0}",  //下载的时候 这里的 data 需要随便传一个 json，否则会报错,  // 你可以根据需要传递参数

            success = (res) =>
            {
                // 解析从云函数返回的结果
                if (res.result != null)
                {
                    Debug.Log(res.result);
                    // 将 JSON 转换为 UserData 对象
                    RankInfo rankInfo = JsonUtility.FromJson<RankInfo>(res.result.ToString());
                    Debug.Log(rankInfo.data[0].gamedata.UserNickName);
                    // 调用传入的成功回调函数，传递 userData
                    successAction?.Invoke(rankInfo);
                }
            },
            fail = (res) =>
            {
                Debug.LogError("GetRankInfo failed: " + res.errMsg);
            },
            complete = (res) =>
            {
                Debug.Log("GetRankInfo complete");
            }
        });
    }

    //获取用户的微信ID和头像,未授权时调用
    public void GetUserWechatIDandAvatar()
    {
        //创建让用户点击授权的按钮
        WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", false);
        btn.Show();
        btn.OnTap((res) =>
        {
            Debug.Log("click userinfo btn: " + JsonUtility.ToJson(res, true));
            if (res.errCode == 0)
            {
                // 用户已允许获取个人信息，返回的 res.userInfo 即为用户信息
                Debug.Log("userinfo: " + JsonUtility.ToJson(res.userInfo, true));
                // 将用户信息存入成员变量，以待后用
               WXUserInfo userInfo = res.userInfo;
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                //CallSetUserData(thisUserData);//上传数据

                // 展示，只是为了测试看到
                AvatarText.text = res.userInfo.avatarUrl;
                NickNameText.text = res.userInfo.nickName;
                Debug.Log($"头像链接为{res.userInfo.avatarUrl}，昵称为{res.userInfo.nickName}");
                //this.ShowUserInfo(res.userInfo.avatarUrl, res.userInfo.nickName);
            }
            else
            {
                Debug.Log("用户拒绝获取个人信息");
            }
            // 最后隐藏授权区域，防止阻塞游戏继续
            btn.Hide();
            Debug.Log("已隐藏热区");
        });
    }


    /// <summary>
    /// 调用Api获取用户信息，已授权之后调用
    /// </summary>
    public  void GetUserInfo()
    {
        WX.GetUserInfo(new GetUserInfoOption()
        {
            lang = "zh_CN",
            success = (res) =>
            {
                Debug.Log("获取用户信息成功(API): " + JsonUtility.ToJson(res.userInfo, true));
                // 将用户信息存入成员变量，或存入云端，方便后续使用
                WXUserInfo userInfo = this.ConvertUserInfo(res.userInfo);
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                // 展示，只是为了测试看到
                ResourceManager.instance.LoadImageFromUrl(res.userInfo.avatarUrl, (texture2D) => {
                    {
                        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        AvatarImage.sprite = sprite;
                    } });
                NickNameText.text = res.userInfo.nickName;
                CreateUserData(thisUserData);//在云数据库中创建元素
                //this.ShowUserInfo(res.userInfo.avatarUrl, res.userInfo.nickName);
            },
            fail = (err) =>
            {
                Debug.Log("获取用户信息失败(API): " + JsonUtility.ToJson(err, true));
            }
        });
    }

    /// <summary>
    /// 将UserInfo对象转为WXUserInfo
    /// ps: 不知为何，相同结构要搞两个对象
    /// </summary>
    /// <param name="userInfo"></param>
    /// <returns></returns>
    WXUserInfo ConvertUserInfo(UserInfo userInfo)
    {
        return new WXUserInfo()
        {
            nickName = userInfo.nickName,
            avatarUrl = userInfo.avatarUrl,
            country = userInfo.country,
            province = userInfo.province,
            city = userInfo.city,
            language = userInfo.language,
            gender = (int)userInfo.gender
        };
    }
}
