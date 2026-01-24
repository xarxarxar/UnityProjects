using SuperScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 祝福仓库界面
/// </summary>
public class BlessStorePanel : MonoBehaviour
{
    public LoopGridView mLoopGridView;
    private bool isInited = false;

    private void Awake()
    {
        if (!isInited)
        {
            mLoopGridView.InitGridView(0, OnGetItemByRowColumn);
            isInited = true;
        }

    }

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// 初始化皮肤面板
    /// </summary>
    public void Init()
    {

        // 确保 InitGridView 已经执行
        if (!isInited)
        {
            mLoopGridView.InitGridView(0, OnGetItemByRowColumn);
            isInited = true;
        }

        // 更新数量
        mLoopGridView.SetListItemCount(BlessDataManager.Instance.AllBless.Count*3);
        // 刷新显示区域
        mLoopGridView.RefreshAllShownItem();
    }

    LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
    {
        if (index < 0 )
        {
            return null;
        }
        // 从池中获取Item（name要与预制体名一致）
        LoopGridViewItem item = gridView.NewListViewItem("祝福item");
        // 设置数据
        BlessItem itemScript = item.GetComponent<BlessItem>();
        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            //itemScript.Init(OnSkinClicked);
        }

        int blessIndex = index / 3;
        int rarity = index % 3;
        Bless bless = BlessDataManager.Instance.AllBless[blessIndex];

        if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(bless.ID, out var counts))
        {
            DataManager.Instance.PlayerInfo.SetBlessCount(bless.ID, new List<int> { -1, 10, 0 });
            counts = DataManager.Instance.PlayerInfo.BlessCount[bless.ID];
        }
        int count = counts[rarity];

        //否则只需要更新就行
        itemScript.SetItem(bless, rarity, count);
        return item;
    }

}
