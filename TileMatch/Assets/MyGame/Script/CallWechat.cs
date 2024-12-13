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
            }
        );

        var feedback = "https://7465-test01cloud-8g9b0glp7aab2737-1322886618.tcb.qcloud.la/mainFolder/Fonts/Source_Han_Sans_SC_Normal_Normal.otf?sign=4665881d91f1c1b11adbb160cad2ced5&t=1729609199";
        WX.GetWXFont(feedback, (font) =>
        {
            wxFont = font;
            //����΢������
            TextMeshProUGUI[] allTexts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
            // ���������ʲ�
            TMP_FontAsset wxFontAsset = TMP_FontAsset.CreateFontAsset(font);

            for (int i = 0; i < allTexts.Length; i++)
            {
                allTexts[i].font = wxFontAsset;
            }
        });

    }

    public  static void ShareApp(UnityAction callback)
    {
        // ������������ͨѶ¼�ķ���
        WX.ShareAppMessage(new ShareAppMessageOption());
        System.Action<OnShowListenerResult> res = null;
        res = (result) =>
        {
            // ��ִ�д���Ļص�
            callback();

            // ��ȡ�� OnShow ����
            WX.OffShow(res);
        };

        // ���� OnShow ����
        WX.OnShow(res);
    }

    /// <summary>
    /// �ϴ��û�����
    /// </summary>
    public void UploadUserData()
    {

    }

    /// <summary>
    /// �����û�����
    /// </summary>
    public void DownloadUserData()
    {

    }

    /// <summary>
    /// �����û�����
    /// </summary>
    private void CreateUserData()
    {
        
    }

}
