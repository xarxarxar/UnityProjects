using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ToggleItemAndData
{
    public DecalData mDecalData;
    public bool mIsToggleOn;

    public ToggleItemAndData(DecalData decalData, bool isToggleOn)
    {
        mDecalData= decalData;
        mIsToggleOn= isToggleOn;
    }
}

public class ToggleItem : MonoBehaviour
{
    public ToggleItemAndData mDataAndTggle;
    DecalData mDecalData;//该toggle对应的decalData
    public static event UnityAction<DecalData> OnToggleClick;//切换到该Toggle的事件
    public static event UnityAction<ToggleItem,DecalData> OnEquipButtonClick;//装备该decal的事件
    private static event UnityAction<int> OnToggleClickIndex;//点击某个Toggle
    private Button mEquipButton;//装备按钮
    [SerializeField] int mIndex;
    public Toggle mToggle;

    public void Init(ToggleItemAndData decalData)
    {
        mDataAndTggle= decalData;
        mDecalData = mDataAndTggle.mDecalData;
        mToggle.onValueChanged.AddListener(ToggleClick);//为toggle添加变化响应事件
        OnToggleClickIndex += OnToggleChange;
    }

    //更新显示
    public void UpdateUI(ToggleItemAndData decalData, int index)
    {
        mIndex= index;

        gameObject.name=index.ToString();
        mDataAndTggle = decalData;
        mDecalData = mDataAndTggle.mDecalData;
        transform.Find("mask").gameObject.SetActive(!mDecalData.isOwned);//已获得
        transform.Find("是否已装备Label").gameObject.SetActive(mDecalData.isEquip);//已装备
        if (mDecalData.isEquip) 
        {
            SkinManager.equipItem = this;//这个就是装备的那个
            SkinManager.equipDecal = mDecalData;
        } 

        transform.Find("是否新获得").gameObject.SetActive(mDecalData.isNewest);//是新获得的

        var icon = transform.Find("Icon").GetChild(0)?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = mDecalData.GetThumbnail();
        }

        mEquipButton = transform.Find("装备Button").GetComponent<Button>();
        mEquipButton.gameObject.SetActive(mDecalData.isOwned && !mDecalData.isEquip && mToggle.isOn);

        mToggle.isOn = mDataAndTggle.mIsToggleOn;
    }

    private void ToggleClick(bool isOn)
    {
        mDataAndTggle.mIsToggleOn = isOn;
        if (isOn)
        {
            //取消最新状态
            mDecalData.isNewest = false;
            transform.Find("是否新获得").gameObject.SetActive(mDecalData.isNewest);
            //已拥有但未装备
            mEquipButton= transform.Find("装备Button").GetComponent<Button>();
            //Debug.Log($"皮肤的id为{mDecalData.id}，isOwned为{mDecalData.isOwned}，isEquip为{mDecalData.isEquip}");
            mEquipButton.gameObject.SetActive(mDecalData.isOwned&& !mDecalData.isEquip);
            mEquipButton.onClick.AddListener(EquipButtonClick);

            //Toggle点击响应事件
            OnToggleClick?.Invoke(mDecalData);
            OnToggleClickIndex?.Invoke(mIndex);
        }
        else
        {
            mEquipButton.onClick.RemoveListener(EquipButtonClick);
            transform.Find("装备Button").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击"装备"按钮时
    /// </summary>
    private void EquipButtonClick()
    {
        mDecalData.isEquip = true;
        mEquipButton.gameObject.SetActive(false);
        OnEquipButtonClick?.Invoke(this, mDecalData);
    }


    private void OnToggleChange(int index)
    {
        mToggle.isOn = mIndex== index;
    }
}
