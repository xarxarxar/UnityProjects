using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有的回调函数
/// </summary>
public class CallBacks : MonoBehaviour
{
    private void Start()
    {
        CardManager.successEvent += GameSuccessCallback;    
    }



    /// <summary>
    /// 游戏成功之后跳出的面板
    /// </summary>
    public GameObject GameSuccessPanel;
    /// <summary>
    /// 爆炸特效面板
    /// </summary>
    public GameObject ExplosionPanel;
    /// <summary>
    /// 游戏成功回调函数
    /// </summary>
    public void GameSuccessCallback()
    {
        GameSuccessPanel.SetActive(true);
        ExplosionPanel.SetActive(true);
    }

    /// <summary>
    /// 游戏失败回调函数
    /// </summary>
    public void GameFailedCallback()
    {

    }
}
