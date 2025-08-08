using DG.Tweening;
using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class BuildingChoosePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollPanel;
    [SerializeField] private Button _toggleButton;
    [SerializeField] private LoopListView2 _loopListView;

    private bool _isExpanded = false;

    private void Start()
    {
        // 初始收起状态
        _scrollPanel.localScale = new Vector3(1, 0, 1);

        // 绑定按钮点击事件
        _toggleButton.onClick.AddListener(TogglePanel);

        // 初始化
        _loopListView.InitListView(10, OnGetItemByIndex);
    }

    // 当需要显示新的 Item 时会调用
    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= 50) return null;

        // 从池中获取Item（name要与预制体名一致）
        LoopListViewItem2 item = listView.NewListViewItem("基础形态");

        // 设置数据
        //ScienceNodeItem script = item.GetComponent<ScienceNodeItem>();

        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            //script.Init(ScienceManager.Instance.GetScienceDataByIndex(index), index);
            return item;
        }
        //否则只需要更新就行
        //script.UpdateUI(ScienceManager.Instance.GetScienceDataByIndex(index), index);

        return item;
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
