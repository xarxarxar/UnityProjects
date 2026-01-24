using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    //UI物体
    public Transform mainPanel;//主界面

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// 显示主界面
    /// </summary>
    public void ShowMainPanel()
    {
        mainPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏主界面
    /// </summary>
    public void HideMainPanel()
    {
        mainPanel.gameObject.SetActive(false);
    }
}
