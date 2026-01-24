using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevelopCanvas : MonoBehaviour
{
    public GameObject DevelopPanel;//开发面板
    public Button OpenDevelopPanel;//开启开发者面板
    public Button CloseDevelopPanel;//开启开发者面板

    public Button DiamonReduce;
    public Button DiamonIncrease;
    public Button CrawnReduce;
    public Button CrawnIncrease;
    public Button PassCountReduce;
    public Button PassCountIncrease;
    public Button ScienceReduce;
    public Button TowerReduce;
    public Button CoinReduce;
    public Button CoinIncrease;
    public Button FourthTimeGameSpeed;
    public Button AllTextIncrease;
    public Button AllTextReduce;
    public Button GetRandomText;
    public Button GetAllPiece;

    void Start()
    {
        OpenDevelopPanel.onClick.AddListener(OnOpen);
        CloseDevelopPanel.onClick.AddListener(OnClose);

        DiamonReduce.onClick.AddListener(OnDiamonReduce);
        DiamonIncrease.onClick.AddListener(OnDiamonIncrease);
        CrawnReduce.onClick.AddListener(OnCrawnReduce);
        CrawnIncrease.onClick.AddListener(OnCrawnIncrease);
        PassCountReduce.onClick.AddListener(OnPassCountReduce);
        PassCountIncrease.onClick.AddListener(OnPassCountIncrease);
        ScienceReduce.onClick.AddListener(OnScienceReduce);
        TowerReduce.onClick.AddListener(OnTowerReduce);
        CoinReduce.onClick.AddListener(OnCoinReduce);
        CoinIncrease.onClick.AddListener(OnCoinIncrease);
        FourthTimeGameSpeed.onClick.AddListener(OnFourthTimeGameSpeed);
        AllTextIncrease.onClick.AddListener(OnAllTextIncrease);
        AllTextReduce.onClick.AddListener(OnAllTextReduce);
        GetRandomText.onClick.AddListener(OnGetRandomText);
        GetAllPiece.onClick.AddListener(AllPieceIncrease);
    }

    void OnOpen()
    {
        DevelopPanel.gameObject.SetActive(true);
    }
    void OnClose()
    {
        DevelopPanel.gameObject.SetActive(false);
    }

    // ------- 以下是所有按钮的空壳方法 -------
    void OnDiamonReduce()
    {
        TipManager.Instance.ShowTip("钻石-5");
        DataManager.Instance.PlayerInfo.SetDiamond(DataManager.Instance.PlayerInfo.Diamond - 5);
    }

    void OnDiamonIncrease()
    {
        TipManager.Instance.ShowTip("钻石+10");
        DataManager.Instance.PlayerInfo.SetDiamond(DataManager.Instance.PlayerInfo.Diamond + 10);
    }

    void OnCrawnReduce()
    {
        TipManager.Instance.ShowTip("王冠-1");
        DataManager.Instance.PlayerInfo.SetCrown(DataManager.Instance.PlayerInfo.Crown - 1);
    }

    void OnCrawnIncrease()
    {
        TipManager.Instance.ShowTip("王冠+1");
        DataManager.Instance.PlayerInfo.SetCrown(DataManager.Instance.PlayerInfo.Crown + 1);
    }

    void AllPieceIncrease()
    {
        TipManager.Instance.ShowTip("所有碎片+1");
        foreach (var towerdata in TowerDataManager.Instance.TowerDatas)
        {
            DataManager.Instance.PlayerInfo.TowerStateMap[towerdata.ID].SetPieceCount(DataManager.Instance.PlayerInfo.TowerStateMap[towerdata.ID].PieceCount+1);
            foreach(var skin in DataManager.Instance.PlayerInfo.TowerStateMap[towerdata.ID].AllSkins)
            {
                skin.Value.SetPieceCount(skin.Value.PieceCount+1);
            }
        }

        foreach (var towerPlatformdata in TowerPlatformDataManager.Instance.TowerPlatformDatas)
        {
            DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformdata.ID].SetPieceCount(DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformdata.ID].PieceCount+1);
            foreach (var skin in DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformdata.ID].AllSkins)
            {
                skin.Value.SetPieceCount(skin.Value.PieceCount + 1);
            }
        }
    }

    void OnPassCountReduce()
    {
        TipManager.Instance.ShowTip("通关次数-1");
        DataManager.Instance.PlayerInfo.SetTotalPassCount(DataManager.Instance.PlayerInfo.TotalPassCount - 1);
    }

    void OnPassCountIncrease()
    {
        TipManager.Instance.ShowTip("通关次数+1");
        DataManager.Instance.PlayerInfo.SetTotalPassCount(DataManager.Instance.PlayerInfo.TotalPassCount+1);
    }

    void OnScienceReduce()
    {
        TipManager.Instance.ShowTip("科技清零");
        DataManager.Instance.PlayerInfo.SetUnlockCount(0);
    }

    void OnTowerReduce()
    {
        TipManager.Instance.ShowTip("炮塔数据清零");
        
    }

    void OnCoinReduce()
    {
        if (CurrencyManager.Instance != null)
        {
            if (CurrencyManager.Instance.HasEnoughGold(100))
            {
                CurrencyManager.Instance.SpendCoin(100);
                TipManager.Instance.ShowTip("金币-100");
            }
            else
            {
                TipManager.Instance.ShowTip("金币不足100");
            }
        }
        else
        {
            TipManager.Instance.ShowTip("战斗未开始");
        }
    }

    void OnCoinIncrease()
    {
        Debug.Log("点击：Coin 增加");
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoin(1000);
            TipManager.Instance.ShowTip("金币+1000");
        }
        else
        {
            TipManager.Instance.ShowTip("战斗未开始");
        }
    }

    void OnFourthTimeGameSpeed()
    {
        Debug.Log("点击：四倍速游戏速度");
        if (CurrencyManager.Instance != null)
        {
            BattleManager.Instance.GameSpeed.Value = 4;
            TipManager.Instance.ShowTip("四倍速已开启");
        }
        else
        {
            TipManager.Instance.ShowTip("战斗未开始");
        }


    }

    void OnAllTextIncrease()
    {
        // 先缓存要修改的键
        List<int> keys = new List<int>();

        foreach (var kv in DataManager.Instance.PlayerInfo.MonthlyCollection)
        {
            keys.Add(kv.Key);
        }

        // 遍历 keys 来修改原字典
        foreach (var key in keys)
        {
            int oldValue = DataManager.Instance.PlayerInfo.MonthlyCollection[key];
            DataManager.Instance.PlayerInfo.SetMonthlyCollection(key,oldValue+1);
        }
        TipManager.Instance.ShowTip("增加成功");
    }

    void OnAllTextReduce()
    {
        // 先缓存要修改的键
        List<int> keys = new List<int>();

        foreach (var kv in DataManager.Instance.PlayerInfo.MonthlyCollection)
        {
            keys.Add(kv.Key);
        }

        // 遍历 keys 来修改原字典
        foreach (var key in keys)
        {
            int oldValue = DataManager.Instance.PlayerInfo.MonthlyCollection[key];
            DataManager.Instance.PlayerInfo.SetMonthlyCollection(key, oldValue - 1);

            if (DataManager.Instance.PlayerInfo.MonthlyCollection[key] < 0)
            {
                DataManager.Instance.PlayerInfo.SetMonthlyCollection(key,0);
            }
        }
        TipManager.Instance.ShowTip("减少成功");
    }

    void OnGetRandomText()
    {
        int index = CollectionManager.GetRandomCharacter();
        Color32 color32=CollectionManager.Instance.GetColorByIndex(index);
        int oldValue = DataManager.Instance.PlayerInfo.MonthlyCollection[index];
        DataManager.Instance.PlayerInfo.SetMonthlyCollection(index, oldValue + 1);
        // 转成 #RRGGBB 字符串
        string hexColor = Color32ToHex(color32);

        TipManager.Instance.ShowTip(
            $"获得<color={hexColor}>{CollectionManager.Chars[index]}</color>"
        );
    }

    public static string Color32ToHex(Color32 color)
    {
        return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
    }
}
