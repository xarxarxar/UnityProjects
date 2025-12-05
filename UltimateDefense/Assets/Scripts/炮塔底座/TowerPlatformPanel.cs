using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPlatformPanel : MonoBehaviour
{
    private int currentIndex = 0;
    private int currentSelectedIndex => TowerPlatformDataManager.Instance.GetTowerPlatformDataIndex(DataManager.Instance.PlayerInfo.CurrentTowerPlatformID.Value);//当前选中的炮塔的ID在TowerDataManager.Instance.towerDatas中的index

    [Header("UI元素")]
    [SerializeField] private Button SelectButton;//选择当前的炮塔
    [SerializeField] private SkinPanel skinPanel;
    [SerializeField] private Image showImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button preButton;
    [SerializeField] private Text towerDesText;

    //按钮样式颜色
    private readonly Color32 lockedColor = new Color32(195, 188, 195, 255);
    private readonly Color32 normalColor = new Color32(248, 187, 41, 255);
    private readonly Color32 selectedColor = new Color32(125, 183, 44, 255);
    private readonly Color32 lockedTextColor = new Color32(117, 109, 124, 255);
    private readonly Color32 normalTextColor = new Color32(151, 53, 4, 255);
    private readonly Color32 selectedTextColor = new Color32(4, 90, 57, 255);

    private void OnEnable()
    {
        currentIndex = currentSelectedIndex;//当前使用的炮塔
        SetTowerIndex(currentIndex);
    }

    private void Start()
    {
        SelectButton.onClick.AddListener(() => SelectTower(currentIndex));

        nextButton.onClick.AddListener(() => SetTowerIndex(currentIndex + 1));
        preButton.onClick.AddListener(() => SetTowerIndex(currentIndex - 1));

    }

    // 切换炮塔索引的方法
    private void SetTowerIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= TowerPlatformDataManager.Instance.TowerPlatformDatas.Count)
            return;

        currentIndex = newIndex;
        preButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < TowerPlatformDataManager.Instance.TowerPlatformDatas.Count - 1);

        UpdateTowerInfo();
        SetSelectButton(currentIndex);
    }

    // 更新UI显示炮塔内容
    private void UpdateTowerInfo()
    {
        TowerPlatformData data = TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex];
        showImage.sprite = TowerPlatformDataManager.Instance.GetCurrentSkin(data).sprite;
        towerDesText.text = data.TowerDescription;

        skinPanel.Init(data.SkinData,DataManager.Instance.PlayerInfo.TowerPlatformStateMap[data.ID], OnSkinChanged);
    }
    

    private void OnSkinChanged(Sprite sprite)
    {
        showImage.sprite = sprite;
    }


    private void SelectTower(int index)
    {
        int towerId = TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID;

        bool isUnlocked = DataManager.Instance.PlayerInfo.TowerPlatformStateMap.ContainsKey(towerId)
                        && DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].IsUnlocked;

        bool isSelected = index == currentSelectedIndex;


        if (!isUnlocked || isSelected)//未解锁或者已经是当前使用的炮塔
        {
            return;
        }
        DataManager.Instance.PlayerInfo.CurrentTowerPlatformID.Value = TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID;
        SetSelectButton(index);

    }
    //设置选择按钮的状态
    private void SetSelectButton(int index)
    {
        int towerId = TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID;

        bool isUnlocked = DataManager.Instance.PlayerInfo.TowerPlatformStateMap.ContainsKey(towerId)
                        && DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].IsUnlocked;


        bool isSelected = index == currentSelectedIndex;

        Image btnImage = SelectButton.GetComponent<Image>();
        Text text = SelectButton.transform.Find("Text").GetComponent<Text>();

        if (!isUnlocked)
        {
            // 未解锁
            btnImage.color = lockedColor;
            text.color = lockedTextColor;
            text.text = "未解锁";
        }
        else if (isSelected)
        {
            // 已解锁 + 已选中
            btnImage.color = selectedColor;
            text.color = selectedTextColor;
            text.text = "已选中";
        }
        else
        {
            // 已解锁 + 未选中
            btnImage.color = normalColor;
            text.color = normalTextColor;
            text.text = "选择";
        }
    }
}
