using System;
using System.Collections.Generic;
using UnityEngine;
using WeChatWASM;
using UnityEngine.UI;

public class CallWechat : MonoBehaviour
{
    /// <summary>
    /// ����ģʽʵ��
    /// </summary>
    public static CallWechat instance;

    /// <summary>
    /// ��ǰ�û�����
    /// </summary>
    public UserData thisUserData = new UserData();

    /// <summary>
    /// �����ݿ��л�ȡ�����п�������
    /// </summary>
    public List<ItemContent> allItems = new List<ItemContent>();

    private void Awake()
    {
        instance = this; // ��ʼ������
    }

    private void Start()
    {
        // ��ʼ���û�����
        thisUserData = new UserData
        {
            UserNickName = "def",
            AvatarURL = "123",
            UserScore = 20,
            cardData = new List<CardData>()
        };

        for (int i = 0; i < allItems.Count; i++)
        {
            thisUserData.cardData.Add(new CardData(allItems[i].Name, 0, allItems[i].Weight, 0));
        }

        // ��ʼ��΢�� SDK
        WX.InitSDK(
            (code) =>
            {
                // ��ʼ���ƻ���
                WX.cloud.Init(new CallFunctionInitParam()
                {
                    env = "test01cloud-8g9b0glp7aab2737", // �ƻ��� ID
                    traceUser = false
                });
                
                // ��ȡ�û���Ϣ����Ȩ��
                GetUserInfo();

                // ��ʾ����˵�
                ShowShareMenuOption ssmo = new ShowShareMenuOption();
                ssmo.menus = new string[] { "shareAppMessage", "shareTimeline" };
                //WX.ShowShareMenu(ssmo); 

                // ������������ͨѶ¼�ķ���
                WX.ShareAppMessage(new ShareAppMessageOption());

                // ��С��Ϸ�Ӻ�̨ת��ǰ̨ʱ���õķ���
                WX.OnShow((res) =>
                {
                    Debug.Log("��Ϸ�Ӻ�̨�ص�ǰ̨");
                });
            }
        );
    }

    /// <summary>
    /// �����ݿ��д����û����ݣ�����Ѿ������򲻴���
    /// </summary>
    /// <param name="gameUserData">��Ϸ�û�����</param>
    public void CreateUserData(UserData gameUserData)
    {
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "create_userdata",
            data = JsonUtility.ToJson(gameUserData), // �û�������תΪ JSON

            success = (res) =>
            {
                Debug.Log("�����û����ݳɹ�");
                GetCardData((res) => { }); // �����ݿ��ȡ�û���Ϣ��������Ϸ��Ϣ
            },
            fail = (res) =>
            {
                Debug.Log("�����û�����ʧ��");
            },
            complete = (res) =>
            {
                Debug.Log("�����û����ݲ������");
            }
        });
    }

    /// <summary>
    /// �ϴ��û����ݵ��ƶ�
    /// </summary>
    /// <param name="gameUserData">��Ϸ�û�����</param>
    public void CallSetUserData(UserData gameUserData)
    {
        Debug.Log("�����ϴ��û�����");
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "upload-userdata",
            data = JsonUtility.ToJson(gameUserData), // �û�������תΪ JSON

            success = (res) =>
            {
                Debug.Log("�ϴ��û����ݳɹ�");
            },
            fail = (res) =>
            {
                Debug.Log("�ϴ��û�����ʧ��");
            },
            complete = (res) =>
            {
                Debug.Log("�ϴ��û����ݲ������");
            }
        });
    }

    /// <summary>
    /// �������ݿ��ȡ��������
    /// </summary>
    /// <param name="successAction">��ȡ�ɹ���Ļص�����</param>
    public void GetCardData(Action<OnlineUserdata> successAction)
    {
        LoadingManager.Instance.StartLoading();

        OnlineUserdata userData = null; // ���ڴ洢�û����ݵı���

        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",
            data = "{\"player_data\":0}", // ����ʱ��Ҫ��㴫һ�� JSON������ᱨ��

            success = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("��ȡ�������ݳɹ�");

                // �������ƺ������صĽ��
                if (res.result != null)
                {
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());
                    Debug.Log($"�û�����Ϊ��{userData.data.cardData}");
                    thisUserData.UserScore = userData.data.UserScore;
                    thisUserData.cardData = userData.data.cardData;

                    for (int i = 0; i < thisUserData.cardData.Count; i++)
                    {
                        Debug.Log($"����������{thisUserData.cardData[i].Count}");
                    }
                    successAction?.Invoke(userData);
                }
            },
            fail = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.LogError("��ȡ��������ʧ�ܣ�" + res.errMsg);
            },
            complete = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("��ȡ�������ݲ������");
            }
        });
    }

    /// <summary>
    /// ��ȡ���а���Ϣ
    /// </summary>
    /// <param name="successAction">��ȡ�ɹ���Ļص�����</param>
    public void GetRankInfo(Action<RankInfo> successAction)
    {
        LoadingManager.Instance.StartLoading();
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-rankinfo",
            data = "{\"player_data\":0}", // ����ʱ��Ҫ��㴫һ�� JSON������ᱨ��

            success = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                if (res.result != null)
                {
                    Debug.Log($"���а����ݣ�{res.result}");
                    RankInfo rankInfo = JsonUtility.FromJson<RankInfo>(res.result.ToString());
                    Debug.Log($"�û��ǳƣ�{rankInfo.data[0].gamedata.UserNickName}");
                    successAction?.Invoke(rankInfo);
                }
            },
            fail = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.LogError("��ȡ���а���Ϣʧ�ܣ�" + res.errMsg);
            },
            complete = (res) =>
            {
                LoadingManager.Instance.StopLoading();
                Debug.Log("��ȡ���а���Ϣ�������");
            }
        });
    }

    /// <summary>
    /// ��ȡ�û���΢��ID��ͷ��δ��Ȩʱ���ã�
    /// </summary>
    public void GetUserWechatIDandAvatar()
    {
        // ������Ȩ��ť
        WXUserInfoButton btn = WX.CreateUserInfoButton(0, 0, Screen.width, Screen.height, "zh_CN", false);
        btn.Show();
        btn.OnTap((res) =>
        {
            if (res.errCode == 0)
            {
                // �û��������ȡ������Ϣ
                Debug.Log("�û���Ϣ��" + JsonUtility.ToJson(res.userInfo, true));
                WXUserInfo userInfo = res.userInfo;
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;
            }
            else
            {
                Debug.Log("�û��ܾ���ȡ������Ϣ");
            }
            btn.Hide();
            Debug.Log("��������Ȩ����");
        });
    }

    /// <summary>
    /// ����API��ȡ�û���Ϣ������Ȩ����ã�
    /// </summary>
    public void GetUserInfo()
    {
        WX.GetUserInfo(new GetUserInfoOption()
        {
            lang = "zh_CN",
            success = (res) =>
            {
                Debug.Log("��ȡ�û���Ϣ�ɹ���API����" + JsonUtility.ToJson(res.userInfo, true));
                WXUserInfo userInfo = ConvertUserInfo(res.userInfo);
                thisUserData.UserNickName = res.userInfo.nickName;
                thisUserData.AvatarURL = res.userInfo.avatarUrl;

                CreateUserData(thisUserData);
            },
            fail = (res) =>
            {
                Debug.Log("��ȡ�û���Ϣʧ��");
            }
        });
    }

    /// <summary>
    /// ��JSON�������û���ϢתΪC#�����
    /// </summary>
    /// <param name="jsonObject">JSON��ʽ���û���Ϣ</param>
    /// <returns>��������û���Ϣ�����</returns>
    private WXUserInfo ConvertUserInfo(object jsonObject)
    {
        return JsonUtility.FromJson<WXUserInfo>(JsonUtility.ToJson(jsonObject));
    }
}
