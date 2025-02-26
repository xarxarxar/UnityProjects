using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;
    private void Awake()
    {
        instance = this;
    }
    //主界面
    public Button startGameButton;//开始挑战游戏按钮

    //按钮
    public Button drawCardButton;//抽牌按钮
    public Button playCardButton;//出牌按钮

    public Button closePanelButton;//点击panel空白的地方关闭当前Panel的按钮

    public void Init()
    {
       
    }

    /// <summary>
    /// 关闭panel
    /// </summary>
    /// <param name="panel"></param>
    public void ClosePanel(GameObject panel)
    {
        panel.transform.DOScale(Vector3.zero, 0.1f);
    }


}
