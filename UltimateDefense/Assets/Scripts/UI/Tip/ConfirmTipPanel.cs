using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmTipPanel : BasePanel
{
    [SerializeField] private GameObject _tipObject;
    [SerializeField] private Text _tiptext;
    [SerializeField] private Text _confirmtext;//确认按钮的那个文本

    /// <summary>
    /// 显示文本提示框，并且让用户点击确认
    /// </summary>
    /// <param name="message">要显示的文本提示，限制在15个字</param>
    /// <param name="confirmText">确认按钮上面的文本</param>
    public void ShowTip(string message,string confirmText="确认")
    {
        _tipObject.SetActive(true);
        _tiptext.text = message;
        _confirmtext.text = confirmText;
    }
}
