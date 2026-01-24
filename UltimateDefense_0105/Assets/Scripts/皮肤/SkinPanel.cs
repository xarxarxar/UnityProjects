using SuperScrollView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkinPanel : MonoBehaviour
{
    public LoopGridView mLoopGridView;
    //private TowerData mTowerData;
    private SkinData mSkinData;
    private ItemState mItemState;
    public int currentSelectedIndex;
    private bool isInited=false;
    private UnityAction<Skin> OnChangeSkin;

    private void Awake()
    {
        if (!isInited)
        {
            mLoopGridView.InitGridView(0, OnGetItemByRowColumn);
            isInited = true;
        }
    }

    /// <summary>
    /// 初始化皮肤面板
    /// </summary>
    public void Init(SkinData skinData,ItemState itemState, UnityAction<Skin> callback)
    {
        mSkinData = skinData;
        mItemState = itemState;
        if (skinData == null || mItemState==null) { return; }

        currentSelectedIndex = TowerDataManager.Instance.GetCurrentSkinIndex(mSkinData,mItemState);

        // 确保 InitGridView 已经执行
        if (!isInited)
        {
            mLoopGridView.InitGridView(0, OnGetItemByRowColumn);
            isInited = true;
        }

        // 更新数量
        mLoopGridView.SetListItemCount(mSkinData.skins.Count);
        // 刷新显示区域
        mLoopGridView.RefreshAllShownItem();

        OnChangeSkin=callback;
    }

    /// <summary>
    /// 刷新显示
    /// </summary>
    public void Refresh()
    {
        // 刷新显示区域
        mLoopGridView.RefreshAllShownItem();
    }

    LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int index, int row, int column)
    {
        if (index < 0 || index >= 50)
        {
            return null;
        }
        // 从池中获取Item（name要与预制体名一致）
        LoopGridViewItem item = gridView.NewListViewItem("皮肤item");
        // 设置数据
        TowerGunSkinItem itemScript = item.GetComponent<TowerGunSkinItem>();
        // 是否是从对象池中第一次拿出来的
        if (item.IsInitHandlerCalled == false)
        {
            item.IsInitHandlerCalled = true;
            itemScript.Init(OnSkinClicked);
        }
        bool isSelected = (index == currentSelectedIndex);
        //否则只需要更新就行
        itemScript.SetItemData(mSkinData.skins[index], index, isSelected, HasSkin(index));
        return item;
    }

    private void OnSkinClicked(int index)
    {
        currentSelectedIndex = index;
        mLoopGridView.RefreshAllShownItem(); // 刷新所有显示项

        if (HasSkin(index))//如果已经解锁了这个皮肤，才保存，否则不保存
        {
            // 保存选择
            mItemState.SetCurrentSkinID(mSkinData.skins[index].ID);
        }
        OnChangeSkin(mSkinData.skins[index]);
    }

    /// <summary>
    /// 是否有这个皮肤
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool HasSkin(int index)
    {
        int skinId= mSkinData.skins[index].ID;
        return TowerDataManager.Instance.HasSkin(mItemState, skinId);
    }
}
