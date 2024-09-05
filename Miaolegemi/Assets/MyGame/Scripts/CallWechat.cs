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
    public Image AvatarImage;//ͷ��ͼƬ
    public Text NickNameText;//�ǳ��ı����     

    //�����ݿ��л�ȡ�����п��ƣ���ʱ��list����
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
                GetUserInfo();//�û���Ȩ��ȡ�ǳƺ�ͷ����Ϣ
                
                //WX.cloud.CallFunction(new CallFunctionParam()
                //{
                //    name = "download-file",  // �滻Ϊ���ȡ������ݵ��ƺ�������
                //    data = "{\"player_data\":0}",  //���ص�ʱ�� ����� data ��Ҫ��㴫һ�� json������ᱨ��,  // ����Ը�����Ҫ���ݲ���

                //    success = (res) =>
                //    {
                //        Debug.Log("GetCardData success");
                //        WX.ShowModal(new ShowModalOption
                //        {
                //            title = "����",
                //            content = res.ToString(),
                //            success = (res) =>
                //            {
                //                if (res.confirm)
                //                {
                //                    Debug.Log("�����ȷ����ť");
                //                    WX.ShowToast(new ShowToastOption
                //                    {
                //                        title = "��Ϣ��ʾ��"
                //                    });
                //                }
                //                else if (res.cancel)
                //                {
                //                    Debug.Log("�����ȡ����ť");
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
                //        title = "����",
                //        content = userdata.UserName,
                //        success = (res) =>
                //        {
                //            if (res.confirm)
                //            {
                //                Debug.Log("�����ȷ����ť");
                //                WX.ShowToast(new ShowToastOption
                //                {
                //                    title = "��Ϣ��ʾ��"
                //                });
                //            }
                //            else if (res.cancel)
                //            {
                //                Debug.Log("�����ȡ����ť");
                //            }
                //        }
                //    });
                //});

                ShowShareMenuOption ssmo = new ShowShareMenuOption();
                ssmo.menus = new string[] { "shareAppMessage", "shareTimeline" };
                //WX.ShowShareMenu(ssmo); // ����ǿ������Ͻ��������Ƿ���Ե���ķ�����������������������ѵķ���������

                // �������������������ͨѶ¼�ķ���������
                WX.ShareAppMessage(new ShareAppMessageOption());

                //���С��Ϸ��̨��ת��̨ǰ���õķ����������ڷ������֮����ø÷���
                WX.OnShow((res) =>
                {
                    Debug.Log("i  m back");
                });

                //CallSetUserData(thisUserData);
                
            }
        );

           }

    //�����ݿ��д���userdata��������еĻ��Ͳ�����
    public void CreateUserData(UserData gameUserData)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "create_userdata",
            //�û�������תΪjson������ʱ���Թ����մ��ᵼ���ƺ�������ԭ���������ô��п����������̽��ҹ���
            data = JsonUtility.ToJson(gameUserData),

            success = (res) =>
            {
                Debug.Log("create UserData success");
                GetCardData((res) => { });//�����ݿ��ȡ�û���Ϣ��������Ϸ��Ϣ
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
            //�û�������תΪjson������ʱ���Թ����մ��ᵼ���ƺ�������ԭ���������ô��п����������̽��ҹ���
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
        // ����һ�����ڴ洢�û����ݵı���
        OnlineUserdata userData = null;

        // �����ƺ��� "TestCloudFunction02" (����������֮ǰ����Ļ�ȡ������ݵ��ƺ�������)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",  // �滻Ϊ���ȡ������ݵ��ƺ�������
            data = "{\"player_data\":0}",  //���ص�ʱ�� ����� data ��Ҫ��㴫һ�� json������ᱨ��,  // ����Ը�����Ҫ���ݲ���

            success = (res) =>
            {
                Debug.Log("GetCardData success");
                
                // �������ƺ������صĽ��
                if (res.result != null)
                {
                    // �� JSON ת��Ϊ UserData ����
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());

                    Debug.Log($"userData is {userData.data.cardData}");
                    thisUserData.UserScore = userData.data.UserScore;
                    thisUserData.cardData = userData.data.cardData;

                    for (int i = 0; i < thisUserData.cardData.Count; i++)
                    {
                        Debug.Log(thisUserData.cardData[i].Count);
                    }
                    // ���ô���ĳɹ��ص����������� userData
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

        // �����ƺ��� "TestCloudFunction02" (����������֮ǰ����Ļ�ȡ������ݵ��ƺ�������)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-rankinfo",  // �滻Ϊ���ȡ������ݵ��ƺ�������
            data = "{\"player_data\":0}",  //���ص�ʱ�� ����� data ��Ҫ��㴫һ�� json������ᱨ��,  // ����Ը�����Ҫ���ݲ���

            success = (res) =>
            {
                // �������ƺ������صĽ��
                if (res.result != null)
                {
                    Debug.Log(res.result);
                    // �� JSON ת��Ϊ UserData ����
                    RankInfo rankInfo = JsonUtility.FromJson<RankInfo>(res.result.ToString());
                    Debug.Log(rankInfo.data[0].gamedata.UserNickName);
                    // ���ô���ĳɹ��ص����������� userData
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

    //��ȡ�û���΢��ID��ͷ��,δ��Ȩʱ����
    public void GetUserWechatIDandAvatar()
    {
        //�������û������Ȩ�İ�ť
        WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", false);
        btn.Show();
        btn.OnTap((res) =>
        {
            Debug.Log("click userinfo btn: " + JsonUtility.ToJson(res, true));
            if (res.errCode == 0)
            {
                // �û��������ȡ������Ϣ�����ص� res.userInfo ��Ϊ�û���Ϣ
                Debug.Log("userinfo: " + JsonUtility.ToJson(res.userInfo, true));
                // ���û���Ϣ�����Ա�������Դ�����
               WXUserInfo userInfo = res.userInfo;
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                //CallSetUserData(thisUserData);//�ϴ�����

                // չʾ��ֻ��Ϊ�˲��Կ���
                AvatarText.text = res.userInfo.avatarUrl;
                NickNameText.text = res.userInfo.nickName;
                Debug.Log($"ͷ������Ϊ{res.userInfo.avatarUrl}���ǳ�Ϊ{res.userInfo.nickName}");
                //this.ShowUserInfo(res.userInfo.avatarUrl, res.userInfo.nickName);
            }
            else
            {
                Debug.Log("�û��ܾ���ȡ������Ϣ");
            }
            // ���������Ȩ���򣬷�ֹ������Ϸ����
            btn.Hide();
            Debug.Log("����������");
        });
    }


    /// <summary>
    /// ����Api��ȡ�û���Ϣ������Ȩ֮�����
    /// </summary>
    public  void GetUserInfo()
    {
        WX.GetUserInfo(new GetUserInfoOption()
        {
            lang = "zh_CN",
            success = (res) =>
            {
                Debug.Log("��ȡ�û���Ϣ�ɹ�(API): " + JsonUtility.ToJson(res.userInfo, true));
                // ���û���Ϣ�����Ա������������ƶˣ��������ʹ��
                WXUserInfo userInfo = this.ConvertUserInfo(res.userInfo);
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                // չʾ��ֻ��Ϊ�˲��Կ���
                ResourceManager.instance.LoadImageFromUrl(res.userInfo.avatarUrl, (texture2D) => {
                    {
                        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
                        AvatarImage.sprite = sprite;
                    } });
                NickNameText.text = res.userInfo.nickName;
                CreateUserData(thisUserData);//�������ݿ��д���Ԫ��
                //this.ShowUserInfo(res.userInfo.avatarUrl, res.userInfo.nickName);
            },
            fail = (err) =>
            {
                Debug.Log("��ȡ�û���Ϣʧ��(API): " + JsonUtility.ToJson(err, true));
            }
        });
    }

    /// <summary>
    /// ��UserInfo����תΪWXUserInfo
    /// ps: ��֪Ϊ�Σ���ͬ�ṹҪ����������
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
