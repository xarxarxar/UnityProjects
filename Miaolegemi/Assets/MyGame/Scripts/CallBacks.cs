using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���еĻص�����
/// </summary>
public class CallBacks : MonoBehaviour
{
    private void Start()
    {
        CardManager.successEvent += GameSuccessCallback;    
    }



    /// <summary>
    /// ��Ϸ�ɹ�֮�����������
    /// </summary>
    public GameObject GameSuccessPanel;
    /// <summary>
    /// ��ը��Ч���
    /// </summary>
    public GameObject ExplosionPanel;
    /// <summary>
    /// ��Ϸ�ɹ��ص�����
    /// </summary>
    public void GameSuccessCallback()
    {
        GameSuccessPanel.SetActive(true);
        ExplosionPanel.SetActive(true);
    }

    /// <summary>
    /// ��Ϸʧ�ܻص�����
    /// </summary>
    public void GameFailedCallback()
    {

    }
}
