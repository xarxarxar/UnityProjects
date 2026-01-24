using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class UnlockCrown //解锁所需的皇冠
{
    public Button ClickButton;
    public Text CountText;
}
[System.Serializable]
class UnlockPiece //解锁所需的碎片
{
    public Button ClickButton;
    public Image ShowImage;
    public Text CountText;
}

public class TowerGunPanel : MonoBehaviour
{
    

    private int currentIndex = 0;
    private int currentSelectedIndex => TowerDataManager.Instance.GetTowerDataIndex(DataManager.Instance.PlayerInfo.CurrentTowerID);//当前选中的炮塔的ID在TowerDataManager.Instance.towerDatas中的index

    [Header("UI元素")]
    [SerializeField] private Button SelectButton;//选择当前的炮塔
    [SerializeField] private Button DescriptionButton;//切换到描述的按钮
    [SerializeField] private Button SkinButton;//切换到皮肤的按钮
    [SerializeField] private TowerDescriptionPanel towerDescriptionPanel;//炮塔介绍面板，包括攻击力，攻速等等
    [SerializeField] private SkinPanel skinPanel;//炮塔皮肤面板
    [SerializeField] private Image showImage;//显示图片
    [SerializeField] private Button nextButton;//切换到下一个炮塔
    [SerializeField] private Button preButton;//切换到上一个炮塔
    [SerializeField] private Text towerDesText;//关于该炮塔的描述文字
    [SerializeField] private UnlockCrown CrownUnlock;//解锁所需的皇冠
    [SerializeField] private UnlockPiece PieceUnlock;//解锁所需的炮塔碎片


    #region 按钮样式
    //按钮样式颜色
    private readonly Color32 lockedColor = new Color32(195, 188, 195, 255);
    private readonly Color32 normalColor = new Color32(248, 187, 41, 255);
    private readonly Color32 selectedColor = new Color32(125, 183, 44, 255);
    private readonly Color32 lockedTextColor = new Color32(117, 109, 124, 255);
    private readonly Color32 normalTextColor = new Color32(151, 53, 4, 255);
    private readonly Color32 selectedTextColor = new Color32(4, 90, 57, 255);
    private readonly Color32 chosenColor = new Color32(246, 225, 156, 255);
    private readonly Color32 unchosenColor = new Color32(255, 255, 255, 255);
    private readonly Color32 enoughColor = new Color32(255, 255, 255, 255);//解锁所需的物品足够时的颜色
    private readonly Color32 notenoughColor = new Color32(204, 69, 2, 255);//解锁所需的物品不足时的颜色
    #endregion
    //属性
    private int Crown => DataManager.Instance.PlayerInfo.Crown;

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
        Debug.Log($"当前的炮塔为{TowerDataManager.Instance.TowerDatas[currentIndex].Name}," +
            $"是否已拥有{DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID].IsUnlocked}");
        Debug.Log(JsonConvert.SerializeObject(DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID], Formatting.Indented));
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
            SetUnlockTowerState(currentIndex);//显示炮塔解锁的条件
        }
        if (skinPanel.gameObject.activeSelf)
        {
            skinPanel.Init(data.SkinData, DataManager.Instance.PlayerInfo.TowerStateMap[data.ID], OnSkinChanged);
            SetUnlockSkinState();
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
        {
            towerDescriptionPanel.Init(TowerDataManager.Instance.TowerDatas[currentIndex]);
            SetUnlockTowerState(currentIndex);//显示炮塔解锁的条件
        }
            
        else
        {
            skinPanel.Init(TowerDataManager.Instance.TowerDatas[currentIndex].SkinData,
                DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID], OnSkinChanged);
            SetUnlockSkinState();
        }
            

        ChooseButton(isDescription ? DescriptionButton : SkinButton);
        SetSelectButton(currentIndex);


    }

    private void OnSkinChanged(Skin skin)
    {
        showImage.sprite= skin.sprite;
        SetUnlockSkinState();
        SetSelectButton(currentIndex);
    }

    // 统一按钮UI状态
    private void ChooseButton(Button chosenButton)
    {
        SetButtonState(DescriptionButton, chosenButton == DescriptionButton);
        SetButtonState(SkinButton, chosenButton == SkinButton);
    }

    private void SetUnlockTowerState(int index)
    {
        TowerData data = TowerDataManager.Instance.TowerDatas[currentIndex];
        if (index == 0)
        {
            CrownUnlock.ClickButton.gameObject.SetActive(false);
            PieceUnlock.ClickButton.gameObject.SetActive(false);
            return;
        }

        CrownUnlock.ClickButton.gameObject.SetActive(true);
        PieceUnlock.ClickButton.gameObject.SetActive(true);

        CrownUnlock.ClickButton.onClick.RemoveAllListeners();
        CrownUnlock.ClickButton.onClick.AddListener(() =>
        {
            GameUIManager.Instance.ShowThing(RewardManager.IconMap[RewardType.Crown], $"皇冠",
                $"可以用于炮塔解锁,炮塔底座解锁,皮肤解锁\n获取途径：商店,通关关卡", true);
        });

        // 若当前处于描述面板，更新内容
        if (towerDescriptionPanel.gameObject.activeSelf)
        {
            CrownUnlock.CountText.text =$"{Crown}/1";
            CrownUnlock.CountText.color = Crown >= 1 ? enoughColor : notenoughColor;

            
            if (DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(data.ID, out ItemState state))
            {
                PieceUnlock.CountText.text =$"{state.PieceCount}/10";
                PieceUnlock.CountText.color = state.PieceCount >= 10 ? enoughColor : notenoughColor;
                PieceUnlock.ShowImage.sprite = data.SkinData.skins[0].sprite;
                PieceUnlock.ClickButton.onClick.RemoveAllListeners();
                PieceUnlock.ClickButton.onClick.AddListener(() =>
                {
                    GameUIManager.Instance.ShowThing(data.SkinData.skins[0].sprite,$"炮塔:{data.name}-碎片",
                        $"{data.TowerDescription}\n获取途径：商店,关卡挑战",true);
                });
            }
        }

    }

    private void SetUnlockSkinState()
    {
        Debug.Log("abc");
        Skin skin = TowerDataManager.Instance.GetSkin(TowerDataManager.Instance.TowerDatas[currentIndex],skinPanel.currentSelectedIndex);
        SkinInfo skinInfo = DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID].AllSkins[skin.ID];
        
        if (TowerDataManager.Instance.TowerDatas[currentIndex].SkinData.skins.IndexOf(skin) == 0)
        {
            CrownUnlock.ClickButton.gameObject.SetActive(false);
            PieceUnlock.ClickButton.gameObject.SetActive(false);
            return;
        }

        CrownUnlock.ClickButton.gameObject.SetActive(true);
        PieceUnlock.ClickButton.gameObject.SetActive(true);

        CrownUnlock.ClickButton.onClick.RemoveAllListeners();
        CrownUnlock.ClickButton.onClick.AddListener(() =>
        {
            GameUIManager.Instance.ShowThing(RewardManager.IconMap[RewardType.Crown], $"皇冠",
                $"可以用于炮塔解锁,炮塔底座解锁,皮肤解锁\n获取途径：商店,通关关卡", true);
        });
        Debug.Log("abc--123");
        // 若当前处于皮肤面板，更新内容
        if (skinPanel.gameObject.activeSelf)
        {
            CrownUnlock.CountText.text =$"{Crown}/1";
            CrownUnlock.CountText.color = Crown >= 1 ? enoughColor : notenoughColor;

            PieceUnlock.CountText.text = $"{skinInfo.PieceCount}/10";
            PieceUnlock.CountText.color = skinInfo.PieceCount >= 10 ? enoughColor : notenoughColor;
            PieceUnlock.ShowImage.sprite = skin.sprite;
            PieceUnlock.ClickButton.onClick.RemoveAllListeners();
            PieceUnlock.ClickButton.onClick.AddListener(() =>
            {
                GameUIManager.Instance.ShowThing(skin.sprite, $"皮肤:{skin.Name}-碎片",
                    $"{skin.description}\n获取途径：商店,关卡挑战", true);
            });
        }
        Debug.Log("def");
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
        Debug.Log($"解锁按钮");
        int towerId = TowerDataManager.Instance.TowerDatas[index].ID;

        bool isUnlocked = false;
        if (DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(towerId, out var towerState))
        {
            isUnlocked = towerState.IsUnlocked;
        }

        bool isSelected = index == currentSelectedIndex;

        //未解锁或者已经是当前使用的炮塔
        if (towerDescriptionPanel.gameObject.activeSelf && isSelected)
        {
            return;
        }
        
        if(!isUnlocked && towerDescriptionPanel.gameObject.activeSelf)//若处于炮塔界面，且未解锁，则进行解锁
        {
            
            if (DataManager.Instance.PlayerInfo.TowerStateMap[towerId].PieceCount>=10 && DataManager.Instance.PlayerInfo.Crown>= 1)
            {
                DataManager.Instance.PlayerInfo.TowerStateMap[towerId].SetPieceCount(DataManager.Instance.PlayerInfo.TowerStateMap[towerId].PieceCount-10);
                DataManager.Instance.PlayerInfo.SetCrown(Crown-1);
                DataManager.Instance.PlayerInfo.TowerStateMap[towerId].SetIsUnlocked(true);
                SetUnlockTowerState(index);
                SetSelectButton(index);
            }
            else
            {
                TipManager.Instance.ShowTip("资源不足");
            }
        }
        else
        {
            DataManager.Instance.PlayerInfo.SetCurrentTower(TowerDataManager.Instance.TowerDatas[index].ID);
        }
        //处于皮肤界面
        if (skinPanel.gameObject.activeSelf)
        {
            Skin skin = TowerDataManager.Instance.GetSkin(TowerDataManager.Instance.TowerDatas[currentIndex], skinPanel.currentSelectedIndex);
            SkinInfo skinInfo = DataManager.Instance.PlayerInfo.TowerStateMap[TowerDataManager.Instance.TowerDatas[currentIndex].ID].AllSkins[skin.ID];
            //该皮肤还没有解锁
            bool needUnlock = !DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(towerId, out var TowerState)
                                || !TowerState.AllSkins.TryGetValue(skin.ID, out var skinState)
                                || !skinState.IsUnlocked;
            if (needUnlock)
            {
                if (skinInfo.PieceCount >= 10 && Crown >= 1)
                {
                    skinInfo.SetPieceCount(Mathf.Max(0, skinInfo.PieceCount - 10));
                    DataManager.Instance.PlayerInfo.SetCrown(Crown-1);
                    DataManager.Instance.PlayerInfo.TowerStateMap[towerId].AllSkins[skin.ID].SetIsUnlocked(true);
                    SetUnlockSkinState();
                    SetSelectButton(index);
                }
                else
                {
                    TipManager.Instance.ShowTip("资源不足");
                }
            }
            else
            {
                DataManager.Instance.PlayerInfo.TowerStateMap[towerId].SetCurrentSkinID(skin.ID);
            }
        }
        SetSelectButton(index);
        skinPanel.Refresh();

    }
    //设置选择按钮的状态
    private void SetSelectButton(int index)
    {
        Debug.Log(JsonConvert.SerializeObject(DataManager.Instance.PlayerInfo.TowerStateMap, Formatting.Indented));
        int towerId = TowerDataManager.Instance.TowerDatas[index].ID;

        bool isUnlocked = false;
        if (DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(towerId, out var towerState))
        {
            isUnlocked = towerState.IsUnlocked;
        }


        bool isSelected = index == currentSelectedIndex;

        Image btnImage = SelectButton.transform.Find("按钮显示").GetComponent<Image>();
        Text text = SelectButton.transform.Find("按钮显示/Text").GetComponent<Text>();

        if (towerDescriptionPanel.gameObject.activeSelf)
        {
            if (!isUnlocked)
            {
                // 未解锁
                btnImage.color = lockedColor;
                text.color = lockedTextColor;
                text.text = "解锁";
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
        if (skinPanel.gameObject.activeSelf)
        {
            Skin skin = TowerDataManager.Instance.GetSkin(TowerDataManager.Instance.TowerDatas[currentIndex], skinPanel.currentSelectedIndex);
            //该皮肤还没有解锁
            bool needUnlock = !DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(towerId, out var TowerState)
                                || !TowerState.AllSkins.TryGetValue(skin.ID, out var skinState)
                                || !skinState.IsUnlocked;
            if (needUnlock)
            {
                Debug.Log($"未解锁");
                // 未解锁
                btnImage.color = lockedColor;
                text.color = lockedTextColor;
                text.text = "解锁";
            }
            else if(DataManager.Instance.PlayerInfo.TowerStateMap[towerId].CurrentSkinID== skin.ID)
            {
                Debug.Log($"已解锁并选中");
                // 已解锁 + 已选中
                btnImage.color = selectedColor;
                text.color = selectedTextColor;
                text.text = "已选中";
            }
            else
            {
                Debug.Log($"已解锁未选中");
                // 已解锁 + 未选中
                btnImage.color = normalColor;
                text.color = normalTextColor;
                text.text = "上场";
            }
            
        }
     }


    
}
