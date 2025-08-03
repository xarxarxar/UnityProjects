using DG.Tweening;
using SuperScrollView;
using System.Collections.Generic;
using UnityEngine;

public class BuildingChoosePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _scrollPanel;
    //[SerializeField] private Button _toggleButton;
    [SerializeField] private LoopListView2 _loopListView;
    [SerializeField] private List<BuildingBase> buildingBases=new List<BuildingBase>(); 

    private bool _isExpanded = false;

    private void Start()
    {
        // 初始化
        _loopListView.InitListView(buildingBases.Count, OnGetItemByIndex);
    }

    // 当需要显示新的 Item 时会调用
    LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
    {
        if (index < 0 || index >= 50) return null;

        // 从池中获取Item（name要与预制体名一致）
        LoopListViewItem2 item = listView.NewListViewItem("基础形态");

        // 设置数据
        BuildingUIInBattle script = item.GetComponent<BuildingUIInBattle>();

        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            script.Init(buildingBases[index], index);
            return item;
        }
        //否则只需要更新就行
        script.UpdateUI(buildingBases[index], index);

        return item;
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
