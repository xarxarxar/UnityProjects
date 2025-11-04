using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TipManager : MonoBehaviour
{
    public static TipManager Instance { get; private set; }
    private Transform _canvasRoot;


    //提示的物体
    [SerializeField]private QuickTipPanel _quickTipPanel;//轻提示，也叫快速提示
    [SerializeField]private ConfirmTipPanel _confirmTipPanel;//确认提示
    [SerializeField]private ChooseTipPanel _chooseTipPanel;//选择提示


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _canvasRoot = GameObject.Find("Canvas")?.transform;
        if (_canvasRoot == null)
        {
            Debug.LogError("未找到 Canvas，UIManager 初始化失败！");
        }
    }

    /// <summary>
    /// QuickTip（轻提示），出现之后几秒钟之后自动消失，字数限制在15个字
    /// </summary>
    /// <param name="message"></param>
    public void ShowTip(string message)
    {
        _quickTipPanel.gameObject.SetActive(true);
        _quickTipPanel.ShowQuickTip(message);
    }

    /// <summary>
    /// 确认提示，需要玩家点击确认,限制在51个字
    /// </summary>
    /// <param name="message">要显示的文本提示，限制在51个字</param>
    /// <param name="confirmText">确认按钮上面的文本</param>
    public void ShowConfirmTip(string message,string confirmText="确认")
    {
        _confirmTipPanel.gameObject.SetActive(true);
        _confirmTipPanel.ShowTip(message, confirmText);
    }

    /// <summary>
    ///选择提示，需要玩家进行选择
    /// </summary>
    /// <param name="message">要显示的文本提示，限制在51个字</param>
    /// <param name="confirmText">确认按钮上面的文本</param>
    public void ShowChooseTip(string message, string confirmText = "确认",string cancelText="取消",
        UnityAction onConfirm = null, UnityAction onCancel = null)
    {
        _chooseTipPanel.gameObject.SetActive(true);
        _chooseTipPanel.ShowTip(message, confirmText,cancelText, onConfirm, onCancel);
    }
}
