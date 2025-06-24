using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guide : MonoBehaviour
{
    public static Guide instance;
    public GameObject tipObject;



    public List<Button> gameCanvasButtons=new List<Button>();//0--暂停，1--PK，2-6分别为大，食，中，无，小

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// 让目标对象在 delay 秒后自动隐藏（SetActive(false)）
    /// </summary>
    public void SetTip(string tip)
    {
        if (tipObject == null) return;

        tipObject.SetActive(true); // 先确保它是激活的
        tipObject.transform.GetChild(0).GetComponent<Text>().text = tip;
    }
    /// <summary>
    /// 让目标对象在 delay 秒后自动隐藏（SetActive(false)）
    /// </summary>
    public void CloseTip()
    {
        if (tipObject == null) return;

        tipObject.SetActive(false); 
    }

    /// <summary>
    /// 设置指定索引的按钮可点击，其余不可点击，并返回可点击按钮列表
    /// </summary>
    /// <param name="activeIndices">需要设置为可点击的按钮索引</param>
    /// <returns>所有被设置为可点击的按钮列表</returns>
    public List<Button> SetButtonActive(params int[] activeIndices)
    {
        List<Button> activeButtons = new List<Button>();

        for (int i = 0; i < gameCanvasButtons.Count; i++)
        {
            Button btn = gameCanvasButtons[i];
            if (btn == null) continue;

            bool isActive = System.Array.IndexOf(activeIndices, i) >= 0;
            btn.interactable = isActive;

            if (isActive)
            {
                activeButtons.Add(btn);
            }
        }

        return activeButtons;
    }

    /// <summary>
    /// 设置指定索引的按钮可显示，其余不显示，并返回可点击按钮列表
    /// </summary>
    /// <param name="activeIndices">需要设置为可点击的按钮索引</param>
    /// <returns>所有被设置为可点击的按钮列表</returns>
    public List<Button> SetButtonVisible(params int[] activeIndices)
    {
        List<Button> activeButtons = new List<Button>();

        for (int i = 0; i < gameCanvasButtons.Count; i++)
        {
            Button btn = gameCanvasButtons[i];
            if (btn == null) continue;

            bool isActive = System.Array.IndexOf(activeIndices, i) >= 0;
            btn.gameObject.SetActive(isActive);

            if (isActive)
            {
                activeButtons.Add(btn);
            }
        }

        return activeButtons;
    }

    //将所有按钮复原
    public void ResetAllButtons()
    {
        foreach (var btn in gameCanvasButtons)
        {
            if (btn != null)
            {
                btn.interactable = true;
                btn.gameObject.SetActive(true); // 如果你同时控制显示，可以保留这行
            }
        }
    }

    /// <summary>
    /// 让目标对象在 delay 秒后自动隐藏（SetActive(false)）
    /// </summary>
    /// <param name="target">要隐藏的 GameObject</param>
    /// <param name="delay">等待的时间（秒）</param>
    public void DeactivateAfterDelay(string tip, float delay)
    {
        if (tipObject == null) return;

        tipObject.SetActive(true); // 先确保它是激活的
        tipObject.GetComponent<Text>().text= tip;
        DOTween.Kill(tipObject);   // 防止重复调用残留的计时器

        DOTween.Sequence()
            .AppendInterval(delay)
            .AppendCallback(() => tipObject.SetActive(false))
            .SetId(tipObject); // 以目标作为 ID，方便管理
    }
}
