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
                new CardData("def", "è����Ƭ1", 30, 3, 0),
                new CardData("def", "è����Ƭ2", 30, 3, 0),
                new CardData("def", "è����Ƭ3", 30, 3, 0),
                new CardData("def", "è����Ƭ4", 30, 3, 0),
                new CardData("def", "è����Ƭ5", 30, 3, 0),
                new CardData("def", "è����Ƭ6", 30, 3, 0),
                new CardData("def", "è����Ƭ7", 30, 3, 0)
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
            //�û�������תΪjson������ʱ���Թ����մ��ᵼ���ƺ�������ԭ���������ô��п����������̽��ҹ���
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
                    Debug.Log(res.result);
                    // �� JSON ת��Ϊ UserData ����
                    userData = JsonUtility.FromJson<OnlineUserdata>(res.result.ToString());
                    Debug.Log(userData.data.UserName);
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
}
