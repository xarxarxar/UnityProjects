using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using WeChatWASM;

public class GetInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �첽��������ȡ�������
    public async void GetCardData(Action<UserData> successAction)
    {
        // ����һ�����ڴ洢�û����ݵı���
        UserData userData = null;

        // �����ƺ��� "TestCloudFunction02" (����������֮ǰ����Ļ�ȡ������ݵ��ƺ�������)
        WX.cloud.CallFunction(new CallFunctionParam()
        {
            name = "get-userdata",  // �滻Ϊ���ȡ������ݵ��ƺ�������
            data = "��ȡ�����Ϣ",  // ����Ը�����Ҫ���ݲ���

            success = (res) =>
            {
                Debug.Log("GetCardData success");

                // �������ƺ������صĽ��
                if (res.result != null)
                {
                    Debug.Log(res.result);
                    // �� JSON ת��Ϊ UserData ����
                    userData = JsonUtility.FromJson<UserData>(res.result.ToString());

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

        // �ȴ��������
        while (userData == null)
        {
            await Task.Yield();  // �ȴ�֡��������ֹ�������߳�
        }
    }
}
