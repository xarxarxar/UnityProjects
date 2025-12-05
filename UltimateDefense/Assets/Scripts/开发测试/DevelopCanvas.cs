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
        int newValue = DataManager.Instance.PlayerInfo.Diamond.Value - 5;
        DataManager.Instance.PlayerInfo.Diamond.Value = Mathf.Max(0, newValue);
        APIAccess.Instance.SaveData();
    }

    void OnDiamonIncrease()
    {
        TipManager.Instance.ShowTip("钻石+10");
        DataManager.Instance.PlayerInfo.Diamond.Value += 10;
        APIAccess.Instance.SaveData();
    }

    void OnCrawnReduce()
    {
        TipManager.Instance.ShowTip("王冠-1");
        int newValue = DataManager.Instance.PlayerInfo.Crown.Value - 1;
        DataManager.Instance.PlayerInfo.Crown.Value = Mathf.Max(0, newValue);
        APIAccess.Instance.SaveData();
    }

    void OnCrawnIncrease()
    {
        TipManager.Instance.ShowTip("王冠+1");
        DataManager.Instance.PlayerInfo.Crown.Value += 1;
        APIAccess.Instance.SaveData();
    }

    void OnPassCountReduce()
    {
        TipManager.Instance.ShowTip("通关次数-1");
        int newValue = DataManager.Instance.PlayerInfo.TotalPassCount.Value - 1;
        DataManager.Instance.PlayerInfo.TotalPassCount.Value = Mathf.Max(0, newValue);
        APIAccess.Instance.SaveData();
    }

    void OnPassCountIncrease()
    {
        TipManager.Instance.ShowTip("通关次数+1");
        DataManager.Instance.PlayerInfo.TotalPassCount.Value += 1;
        APIAccess.Instance.SaveData();
    }

    void OnScienceReduce()
    {
        TipManager.Instance.ShowTip("科技清零");
        DataManager.Instance.PlayerInfo.UnlockCount.Value = 0;
        APIAccess.Instance.SaveData();
    }

    void OnTowerReduce()
    {
        TipManager.Instance.ShowTip("炮塔数据清零");
        
        APIAccess.Instance.SaveData();
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
            DataManager.Instance.PlayerInfo.MonthlyCollection[key]++;
        }
        TipManager.Instance.ShowTip("增加成功");
        APIAccess.Instance.SaveData();
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
            DataManager.Instance.PlayerInfo.MonthlyCollection[key]--;
            if (DataManager.Instance.PlayerInfo.MonthlyCollection[key] < 0)
            {
                DataManager.Instance.PlayerInfo.MonthlyCollection[key] = 0;
            }
        }
        TipManager.Instance.ShowTip("减少成功");
        APIAccess.Instance.SaveData();
    }

    void OnGetRandomText()
    {
        int index = CollectionManager.GetRandomCharacter();
        Color32 color32=CollectionManager.Instance.GetColorByIndex(index);
        DataManager.Instance.PlayerInfo.MonthlyCollection[index]++;
        // 转成 #RRGGBB 字符串
        string hexColor = Color32ToHex(color32);

        TipManager.Instance.ShowTip(
            $"获得<color={hexColor}>{CollectionManager.Chars[index]}</color>"
        );
        APIAccess.Instance.SaveData();
    }

    public static string Color32ToHex(Color32 color)
    {
        return $"#{color.r:X2}{color.g:X2}{color.b:X2}";
    }
}
