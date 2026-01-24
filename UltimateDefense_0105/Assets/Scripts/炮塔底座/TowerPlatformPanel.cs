using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerPlatformPanel : MonoBehaviour
{
    private int currentIndex = 0;
    private int currentSelectedIndex => TowerPlatformDataManager.Instance.GetTowerPlatformDataIndex(DataManager.Instance.PlayerInfo.CurrentTowerPlatformID);//当前选中的炮塔的ID在TowerDataManager.Instance.towerDatas中的index

    [Header("UI元素")]
    [SerializeField] private Button SelectButton;//选择当前的炮塔
    [SerializeField] private SkinPanel skinPanel;
    [SerializeField] private Image showImage;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button preButton;
    [SerializeField] private Text towerDesText;
    [SerializeField] private UnlockCrown CrownUnlock;//解锁所需的皇冠
    [SerializeField] private UnlockPiece PieceUnlock;//解锁所需的炮塔碎片

    //按钮样式颜色
    private readonly Color32 lockedColor = new Color32(195, 188, 195, 255);
    private readonly Color32 normalColor = new Color32(248, 187, 41, 255);
    private readonly Color32 selectedColor = new Color32(125, 183, 44, 255);
    private readonly Color32 lockedTextColor = new Color32(117, 109, 124, 255);
    private readonly Color32 normalTextColor = new Color32(151, 53, 4, 255);
    private readonly Color32 selectedTextColor = new Color32(4, 90, 57, 255);
    private readonly Color32 enoughColor = new Color32(255, 255, 255, 255);//解锁所需的物品足够时的颜色
    private readonly Color32 notenoughColor = new Color32(204, 69, 2, 255);//解锁所需的物品不足时的颜色

    //属性
    private int Crown => DataManager.Instance.PlayerInfo.Crown;

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

        //当前皮肤的选择是第0个，那么就是默认炮塔底座的解锁
        if (TowerPlatformDataManager.Instance.GetCurrentSkinIndex(data) == 0)
        {
            SetUnlockTowerState(currentIndex);//显示炮塔底座解锁的条件
        }
        else
        {
            SetUnlockSkinState();
        }
    }
    

    private void OnSkinChanged(Skin skin)
    {
        showImage.sprite =skin. sprite;
        SetUnlockSkinState();
        SetSelectButton(currentIndex);

        TowerPlatformData data = TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex];
        //当前皮肤的选择是第0个，那么就是默认炮塔底座的解锁
        if (TowerPlatformDataManager.Instance.GetSkinIndex(data, skin) == 0)
        {
            SetUnlockTowerState(currentIndex);//显示炮塔底座解锁的条件
        }
        else
        {
            SetUnlockSkinState();
        }
    }


    private void SelectTower(int index)
    {
        int towerId = TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID;

        bool isUnlocked =DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerId, out var platformState)&& platformState.IsUnlocked;

        bool isSelected = index == currentSelectedIndex;


        //未解锁或者已经是当前使用的炮塔
        if (skinPanel.currentSelectedIndex==0&& isSelected)
        {
            return;
        }

        if (!isUnlocked && skinPanel.currentSelectedIndex == 0)//若处于炮塔界面，且未解锁，则进行解锁
        {

            if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].PieceCount >= 10 && Crown >= 1)
            {
                DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].SetPieceCount(DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].PieceCount-10);
                DataManager.Instance.PlayerInfo.SetCrown(Crown - 1);
                DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].SetIsUnlocked(true);
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
            DataManager.Instance.PlayerInfo.SetCurrentTowerPlatform(TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID);
        }
        //处于皮肤界面
        if (skinPanel.currentSelectedIndex!=0)
        {
            Skin skin = TowerPlatformDataManager.Instance.GetSkin(TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex], skinPanel.currentSelectedIndex);
            SkinInfo skinInfo = DataManager.Instance.PlayerInfo.TowerPlatformStateMap[TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex].ID].AllSkins[skin.ID];
            //该皮肤还没有解锁
            bool needUnlock = !DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerId, out var TowerplatformState)
                                || !TowerplatformState.AllSkins.TryGetValue(skin.ID, out var skinState)
                                || !skinState.IsUnlocked;
            if (needUnlock)
            {
                if (skinInfo.PieceCount >= 10 && Crown >= 1)
                {
                    skinInfo.SetPieceCount(Mathf.Max(0, skinInfo.PieceCount-10));
                    DataManager.Instance.PlayerInfo.SetCrown(Crown - 1);
                    DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].AllSkins[skin.ID].SetIsUnlocked(true);
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
                DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].SetCurrentSkinID(skin.ID);
            }
        }
        SetSelectButton(index);
        skinPanel.Refresh();
    }


    private void SetUnlockTowerState(int index)
    {
        TowerPlatformData data = TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex];
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

        
        CrownUnlock.CountText.text = $"{DataManager.Instance.PlayerInfo.Crown}/1";
        CrownUnlock.CountText.color = DataManager.Instance.PlayerInfo.Crown >= 1 ? enoughColor : notenoughColor;


        if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(data.ID, out ItemState state))
        {
            PieceUnlock.CountText.text = $"{state.PieceCount}/10";
            PieceUnlock.CountText.color = state.PieceCount >= 10 ? enoughColor : notenoughColor;
            PieceUnlock.ShowImage.sprite = data.SkinData.skins[0].sprite;
            PieceUnlock.ClickButton.onClick.RemoveAllListeners();
            PieceUnlock.ClickButton.onClick.AddListener(() =>
            {
                GameUIManager.Instance.ShowThing(data.SkinData.skins[0].sprite, $"炮塔:{data.name}-碎片",
                    $"{data.TowerDescription}\n获取途径：商店,关卡挑战", true);
            });
        }
        

    }

    private void SetUnlockSkinState()
    {
        Skin skin = TowerPlatformDataManager.Instance.GetSkin(TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex], skinPanel.currentSelectedIndex);
        SkinInfo skinInfo = DataManager.Instance.PlayerInfo.TowerPlatformStateMap[TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex].ID].AllSkins[skin.ID];
        Debug.Log($"切换到皮肤面板，皮肤当前的index为{TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex].SkinData.skins.IndexOf(skin)}");
        if (TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex].SkinData.skins.IndexOf(skin) == 0)
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


        CrownUnlock.CountText.text = $"{DataManager.Instance.PlayerInfo.Crown}/1";
        CrownUnlock.CountText.color = DataManager.Instance.PlayerInfo.Crown >= 1 ? enoughColor : notenoughColor;

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


    //设置选择按钮的状态
    private void SetSelectButton(int index)
    {
        int towerId = TowerPlatformDataManager.Instance.TowerPlatformDatas[index].ID;

        bool isUnlocked = DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerId, out var platformState) && platformState.IsUnlocked;


        bool isSelected = index == currentSelectedIndex;

        Image btnImage = SelectButton.transform.Find("按钮显示").GetComponent<Image>();
        Text text = SelectButton.transform.Find("按钮显示/Text").GetComponent<Text>();

        Debug.Log($"index is {index},currentSelectedIndex is {currentSelectedIndex},DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].IsUnlocked is {DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].IsUnlocked}");

        if (skinPanel.currentSelectedIndex == 0)
        {
            if (!isUnlocked)
            {
                // 未解锁
                Debug.Log($"未解锁——1");
                btnImage.color = lockedColor;
                text.color = lockedTextColor;
                text.text = "解锁";
            }
            else if (isSelected)
            {
                // 已解锁 + 已选中
                Debug.Log($"已解锁 + 已选中");
                btnImage.color = selectedColor;
                text.color = selectedTextColor;
                text.text = "已选中";
            }
            else
            {
                // 已解锁 + 未选中
                Debug.Log($" 已解锁 + 未选中");
                btnImage.color = normalColor;
                text.color = normalTextColor;
                text.text = "上场";
            }
        }
        else
        {
            Skin skin = TowerPlatformDataManager.Instance.GetSkin(TowerPlatformDataManager.Instance.TowerPlatformDatas[currentIndex], skinPanel.currentSelectedIndex);
            //该皮肤还没有解锁
            bool needUnlock =!DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(towerId, out var TowerplatformState)
                                || !TowerplatformState.AllSkins.TryGetValue(skin.ID, out var skinState)
                                || !skinState.IsUnlocked;
            if (needUnlock)
            {
                Debug.Log($"未解锁");
                // 未解锁
                btnImage.color = lockedColor;
                text.color = lockedTextColor;
                text.text = "解锁";
            }
            else if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerId].CurrentSkinID == skin.ID)
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
                text.text = "选择";
            }
            
        }
    }
}
