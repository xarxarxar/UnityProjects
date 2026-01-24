using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 用户需要进行选择的提示，“确认”和“取消”
/// </summary>
public class ChooseTipPanel : BasePanel
{
    [SerializeField] private GameObject _tipObject;
    [SerializeField] private Text _tiptext;
    [SerializeField] private Text _confirmtext;//确认按钮的那个文本
    [SerializeField] private Text _canceltext;//确认按钮的那个文本
    [SerializeField] private Button _confirmButton;//确认按钮
    [SerializeField] private Button _cancelButton;//取消按钮

    private UnityAction _onConfirm;//玩家点击确认按钮时
    private UnityAction _onCalcel;//玩家点击取消按钮时
    private UnityAction _onEnd;//面板关闭时

    protected override void InitPanel()
    {
        base.InitPanel();
        _confirmButton.onClick.RemoveListener(ConfirmButtonClicked);
        _confirmButton.onClick.AddListener(ConfirmButtonClicked);

        _cancelButton.onClick.RemoveListener(CancelButtonClicked);
        _cancelButton.onClick.AddListener(CancelButtonClicked);
    }

    /// <summary>
    /// 显示文本提示框，并且让用户点击确认
    /// </summary>
    /// <param name="message">要显示的文本提示，限制在15个字</param>
    /// <param name="confirmText">确认按钮上面的文本</param>
    public void ShowTip(string message, string confirmText = "确认",string cancelText="取消",
        UnityAction onConfirm=null,UnityAction onCancel=null)
    {
        _tipObject.SetActive(true);
        _tiptext.text = message;
        _confirmtext.text = confirmText;
        _canceltext.text = cancelText;

        _onConfirm= onConfirm;
        _onCalcel= onCancel;
    }

    //确认按钮点击时
    private void ConfirmButtonClicked()
    {
        OnCloseButton();
        _onCalcel =null;
    }

    //取消按钮点击时
    private void CancelButtonClicked()
    {
        _onConfirm =null;
    }

    //面板关闭时
    public override void OnEnd()
    {
        base.OnEnd();
        _onConfirm?.Invoke();
        _onCalcel?.Invoke();
        
    }
}
