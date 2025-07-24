using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoosePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollPanel;
    [SerializeField] private Button _toggleButton;

    private bool _isExpanded = false;

    private void Start()
    {
        // 初始收起状态
        _scrollPanel.localScale = new Vector3(1, 0, 1);

        // 绑定按钮点击事件
        _toggleButton.onClick.AddListener(TogglePanel);
    }

    private void TogglePanel()
    {
        if (_isExpanded)
        {
            // 收起
            _scrollPanel.DOScaleY(0f, 0.2f).SetEase(Ease.InCubic);
        }
        else
        {
            // 展开
            _scrollPanel.DOScaleY(1f, 0.2f).SetEase(Ease.OutCubic);
        }

        _isExpanded = !_isExpanded;
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowPanel()
    {
        if (_isExpanded) return;
        _isExpanded = true;
        // 展开
        _scrollPanel.DOScaleY(1f, 0.2f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void HidePanel()
    {
        if (!_isExpanded) return;
        _isExpanded = false;
        // 展开
        _scrollPanel.DOScaleY(0f, 0.2f).SetEase(Ease.InCubic);
    }
}
