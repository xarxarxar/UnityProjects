using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting; // DoTween �������ռ�

public class LoadingManager : MonoBehaviour
{
    public GameObject LoadingPanel;//�������
    public static LoadingManager Instance;

    private void Awake()
    {
        Instance=this;
    }

    /// <summary>
    /// ��ʼ���ض���
    /// </summary>
    public void StartLoading()
    {
        LoadingPanel.SetActive(true);
    }

    /// <summary>
    /// ֹͣ���ض���
    /// </summary>
    public void StopLoading()
    {
        LoadingPanel.SetActive(false);
    }
}
