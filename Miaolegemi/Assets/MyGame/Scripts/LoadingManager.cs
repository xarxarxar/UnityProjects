using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting; // DoTween 的命名空间

public class LoadingManager : MonoBehaviour
{
    public GameObject LoadingPanel;//加载面板
    public static LoadingManager Instance;

    private void Awake()
    {
        Instance=this;
    }

    /// <summary>
    /// 开始加载动画
    /// </summary>
    public void StartLoading()
    {
        LoadingPanel.SetActive(true);
    }

    /// <summary>
    /// 停止加载动画
    /// </summary>
    public void StopLoading()
    {
        LoadingPanel.SetActive(false);
    }
}
