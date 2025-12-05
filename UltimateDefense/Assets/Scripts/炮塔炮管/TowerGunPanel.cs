using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerGunPanel : MonoBehaviour
{

    private int currentIndex = 0;
    private int currentSelectedIndex => TowerDataManager.Instance.GetTowerDataIndex(DataManager.Instance.PlayerInfo.CurrentTowerID.Value);//当前选中的炮塔的ID在TowerDataManager.Instance.towerDatas中的index

    [Header("UI元素")]
    [SerializeField] private Button SelectButton;//选择当前的炮塔
    [SerializeField] private Button DescriptionButton;
    [SerializeField] private Button SkinButton;
    [SerializeField] private TowerDescriptionPanel towerDescriptionPanel;
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
    private readonly Color32 chosenColor = new Color32(246, 225, 156, 255);
    private readonly Color32 unchosenColor = new Color32(255, 255, 255, 255);

    private void OnEnable()
    {
        currentIndex = currentSelectedIndex;//当前使用的炮塔
        SwitchPanel(true);   // 默认打开描述面板
        SetTowerIndex(currentIndex);
    }

    private void Start()
    {
        SelectButton.onClick.AddListener(()=>SelectTower(currentIndex));

        nextButton.onClick.AddListener(() => SetTowerIndex(currentIndex + 1));
        preButton.onClick.AddListener(() => SetTowerIndex(currentIndex - 1));

        DescriptionButton.onClick.AddListener(() => SwitchPanel(true));
        SkinButton.onClick.AddListener(() => SwitchPanel(false));

    }



    // 切换炮塔索引的方法
    private void SetTowerIndex(int newIndex)
    {
        if (newIndex < 0 || newIndex >= TowerDataManager.Instance.TowerDatas.Count)
            return;

        currentIndex = newIndex;
        preButton.gameObject.SetActive(currentIndex > 0);
        nextButton.gameObject.SetActive(currentIndex < TowerDataManager.Instance.TowerDatas.Count - 1);
        
        UpdateTowerInfo();
        SetSelectButton(currentIndex);
    }

    // 更新UI显示炮塔内容
    private void UpdateTowerInfo()
    {
        TowerData data = TowerDataManager.Instance.TowerDatas[currentIndex];
        showImage.sprite = TowerDataManager.Instance.GetCurrentSkin(data).sprite;
        towerDesText.text = data.TowerDescription;

        // 若当前处于描述面板，更新内容
        if (towerDescriptionPanel.gameObject.activeSelf)
        {
            towerDescriptionPanel.Init(data);
        }
        if (skinPanel.gameObject.activeSelf)
        {
            skinPanel.Init(data.SkinData, DataManager.Instance.PlayerInfo.TowerStateMap[data.ID], OnSkinChanged);
        }
    }

    // 统一处理两个面板的切换
    private void SwitchPanel(bool isDescription)
    {
        bool isSkin = !isDescription;

        towerDescriptionPanel.gameObject.SetActive(isDescription);
        skinPanel.gameObject.SetActive(isSkin);

        Debug.Log($"当前的index为{currentIndex}，TowerDatas长度为{TowerDataManager.Instance.TowerDatas.Count}");
        if (isDescription)
            towerDescriptionPanel.Init(TowerDataManager.Instance.TowerDatas[currentIndex]);
        else
            skinPanel.Init(TowerDataManager.Instance.TowerDatas[currentIndex].SkinData,
                DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID], OnSkinChanged);

        ChooseButton(isDescription ? DescriptionButton : SkinButton);
        
    }

    private void OnSkinChanged(Sprite sprite)
    {
        showImage.sprite=sprite;
    }

    // 统一按钮UI状态
    private void ChooseButton(Button chosenButton)
    {
        SetButtonState(DescriptionButton, chosenButton == DescriptionButton);
        SetButtonState(SkinButton, chosenButton == SkinButton);
    }

    // 抽取重复代码，控制按钮样式
    private void SetButtonState(Button btn, bool isChosen)
    {
        btn.transform.Find("名称Text").GetComponent<Text>().color = isChosen ? chosenColor : unchosenColor;
        btn.transform.Find("选中Image").gameObject.SetActive(isChosen);
        btn.transform.Find("图标Image").GetComponent<Image>().color = isChosen ? chosenColor : unchosenColor;
    }

    private void SelectTower(int index)
    {
        int towerId = TowerDataManager.Instance.TowerDatas[index].ID;

        bool isUnlocked = DataManager.Instance.PlayerInfo.TowerStateMap.ContainsKey(towerId)
                        && DataManager.Instance.PlayerInfo.TowerStateMap[towerId].IsUnlocked;

        bool isSelected = index == currentSelectedIndex;


        if (!isUnlocked||isSelected)//未解锁或者已经是当前使用的炮塔
        {
            return;
        }
        DataManager.Instance.PlayerInfo.CurrentTowerID.Value = TowerDataManager.Instance.TowerDatas[index].ID ;
        SetSelectButton(index);

    }
    //设置选择按钮的状态
    private void SetSelectButton(int index)
    {
        int towerId = TowerDataManager.Instance.TowerDatas[index].ID;

        bool isUnlocked = DataManager.Instance.PlayerInfo.TowerStateMap.ContainsKey(towerId)
                        && DataManager.Instance.PlayerInfo.TowerStateMap[towerId].IsUnlocked;


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
