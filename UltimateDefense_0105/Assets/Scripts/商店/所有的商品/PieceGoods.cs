using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Tower,          // 炮塔
    TowerSkin,      // 炮塔皮肤
    Base,           // 炮塔底座
    BaseSkin,       // 底座皮肤
    Crown,          // 皇冠
    Bless,          // 祝福
}

/// <summary>
/// 碎片商品,包括炮塔碎片，底座碎片，各种皮肤碎片等等
/// </summary>
public class PieceGoods : GoodsItem
{
    //UI
    public Image showImage;

    private TowerData chosedTowerData;
    private TowerPlatformData chosedTowerPlatformData;
    private (TowerData, Skin) chosedTowerSkin;
    private (TowerPlatformData, Skin) chosedTowerPlatformSkin;

    public override void Init()
    {
        
        base.Init();
        chosedTowerData = null;
        chosedTowerPlatformData = null;
        chosedTowerSkin = (null,null);
        chosedTowerPlatformSkin = (null,null);

        GetPiece();
        Price = 50;
        PriceText.text=Price.ToString();

    }

    public override void Click()
    {
        Debug.Log("碎片商品点击");
        if(!isBought)
        {
            //钻石足够
            if (MetaCurrencyManager.Instance.SpendMetaCoin(RewardType.Diamond, Price))
            {
                Buy();
            }
            else
            {
                TipManager.Instance.ShowTip("钻石不足");
            }
        }

        

    }

    public override void Buy()
    {
        
        isBought = true;
        if (chosedTowerData != null)
        {
            if (DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(chosedTowerData.ID, out ItemState state))
            {
                state.SetPieceCount(state.PieceCount+1);
            }
            SetClickButton();
            return;
        }
        
        if (chosedTowerPlatformData != null)
        {
            if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(chosedTowerPlatformData.ID, out ItemState state))
            {
                state.SetPieceCount(state.PieceCount + 1);
            }
            SetClickButton();
            return;
        }

        if (chosedTowerSkin != (null,null))
        {
            TowerData towerData = chosedTowerSkin.Item1;
            Skin skin = chosedTowerSkin.Item2;
            SkinInfo skinInfo=DataManager.Instance.PlayerInfo.TowerStateMap[towerData.ID].AllSkins[skin.ID];
            skinInfo.SetPieceCount(skinInfo.PieceCount+1);
            SetClickButton();
            return;
        }

        if (chosedTowerPlatformSkin != (null, null))
        {
            TowerPlatformData towerPlatformData = chosedTowerPlatformSkin.Item1;
            Skin skin = chosedTowerPlatformSkin.Item2;
            SkinInfo skinInfo = DataManager.Instance.PlayerInfo.TowerPlatformStateMap[towerPlatformData.ID].AllSkins[skin.ID];
            skinInfo.SetPieceCount(skinInfo.PieceCount + 1);
            SetClickButton();
            return;
        }
        TipManager.Instance.ShowTip("购买成功");
        
    }

    private  float TowerWeight = 0.2f;//抽取炮塔的概率
    private float TowerBaseWeight = 0f;//抽取炮塔底座的概率，暂时设置为0
    private float TowerSkinWeight = 0.3f;//抽取炮塔皮肤的概率
    private float TowerBaseSkinWeight = 0f;//抽取炮塔底座皮肤的概率，暂时设置为0
    private float OwnedReduceFactor = 0.5f;   // 玩家已有时概率降低多少

    /// <summary>
    /// 获取一个碎片，可能是炮塔，可能是底座，可能是皮肤,返回的是炮塔还是底座，然后它的ID是什么。如果是皮肤的话，int
    /// </summary>
    /// <returns></returns>
    private void GetPiece()
    {
        float total = TowerWeight + TowerBaseWeight + TowerSkinWeight + TowerBaseSkinWeight;

        float rand = Random.Range(0f, total);

        if (rand < TowerWeight)//抽取炮塔碎片
        {
            chosedTowerData=GetTowerPiece();
            Name = chosedTowerData.Name;
            NameText.text = Name;
            showImage.sprite= chosedTowerData.SkinData.skins[0].sprite;
            return;
        }

        rand -= TowerWeight;

        if (rand < TowerBaseWeight)//抽取炮塔底座碎片
        {
            Debug.Log($"rand=={rand},抽取到炮塔底座");
            chosedTowerPlatformData = GetTowerBasePiece();
            Name = "炮塔底座" + chosedTowerPlatformData.Name;
            NameText.text = Name;
            showImage.sprite = chosedTowerPlatformData.SkinData.skins[0].sprite;
            return;
        }

        rand -= TowerBaseWeight;

        if (rand < TowerSkinWeight)//抽取炮塔皮肤碎片
        {
            chosedTowerSkin = GetTowerSkinPiece();
            Name = "皮肤-" + chosedTowerSkin.Item2.Name;
            NameText.text = Name;
            showImage.sprite = chosedTowerSkin.Item2.sprite;
            return;
        }

        ////抽取炮塔底座皮肤碎片
        //chosedTowerPlatformSkin = GetTowerBaseSkinPiece();
        //Name = "炮塔底座皮肤" + chosedTowerPlatformSkin.Item2.Name;
        //NameText.text = Name;
        //showImage.sprite = chosedTowerPlatformSkin.Item2.sprite;
        return;

    }

    //抽取一个炮塔碎片,返回炮塔的ID
    private TowerData GetTowerPiece()
    {
        // 1. 筛选出所有 ID != 0 的塔（因为 0 是默认拥有的）
        List<TowerData> validTowers = new List<TowerData>();
        foreach (var t in TowerDataManager.Instance.TowerDatas)
        {
            if (t.ID != 0)
                validTowers.Add(t);
        }
        // 防止空列表（理论上不会）
        if (validTowers.Count == 0)
        {
            Debug.LogError("没有可抽取的炮塔（除了ID=0）。");
            return null;
        }

        // 2. 计算权重
        float totalWeight = 0f;
        Dictionary<TowerData, float> weightMap = new Dictionary<TowerData, float>();


        foreach (var t in validTowers)
        {
            // 默认权重 = 1
            float weight = 1f;

            // 如果已经解锁 → 权重减半
            if (DataManager.Instance.PlayerInfo.TowerStateMap.TryGetValue(t.ID, out ItemState state) && state.IsUnlocked)
            {
                weight *= 0.5f;
            }

            weightMap[t] = weight;
            totalWeight += weight;
        }

        // 3. 使用权重随机
        float rand = Random.Range(0f, totalWeight);
        foreach (var kv in weightMap)
        {
            if (rand < kv.Value)
            {
                return kv.Key;  // 抽中了这个 Tower
            }
            rand -= kv.Value;
        }
        // 理论不会到这里
        return validTowers[validTowers.Count - 1];
    }

    //抽取一个炮塔底座碎片,返回炮塔底座的ID
    private TowerPlatformData GetTowerBasePiece()
    {
        // 1. 筛选出所有 ID != 0 的塔（因为 0 是默认拥有的）
        List<TowerPlatformData> validTowers = new List<TowerPlatformData>();
        foreach (var t in TowerPlatformDataManager.Instance.TowerPlatformDatas)
        {
            if (t.ID != 0)
                validTowers.Add(t);
        }
        // 防止空列表（理论上不会）
        if (validTowers.Count == 0)
        {
            Debug.LogError("没有可抽取的炮塔（除了ID=0）。");
            return null;
        }

        // 2. 计算权重
        float totalWeight = 0f;
        Dictionary<TowerPlatformData, float> weightMap = new Dictionary<TowerPlatformData, float>();


        foreach (var t in validTowers)
        {
            // 默认权重 = 1
            float weight = 1f;

            // 如果已经解锁 → 权重减半
            if (DataManager.Instance.PlayerInfo.TowerPlatformStateMap.TryGetValue(t.ID, out ItemState state) && state.IsUnlocked)
            {
                weight *= 0.5f;
            }

            weightMap[t] = weight;
            totalWeight += weight;
        }
        // 3. 使用权重随机
        float rand = Random.Range(0f, totalWeight);
        foreach (var kv in weightMap)
        {
            if (rand < kv.Value)
            {
                return kv.Key;  // 抽中了这个 TowerData
            }
            rand -= kv.Value;
        }
        // 理论不会到这里
        return validTowers[validTowers.Count - 1];

    }

    // 抽取一个炮塔皮肤碎片, 返回 (TowerData, Skin)
    private (TowerData, Skin) GetTowerSkinPiece()
    {
        float totalWeight = 0f;

        // 用 List 临时存，不用 Dictionary
        List<(TowerData tower, Skin skin, float weight)> pool
            = new List<(TowerData, Skin, float)>();

        var towerDatas = TowerDataManager.Instance.TowerDatas;
        var towerStateMap = DataManager.Instance.PlayerInfo.TowerStateMap;

        for (int i = 0; i < towerDatas.Count; i++)
        {
            var tower = towerDatas[i];

            if (tower == null || tower.SkinData == null || tower.SkinData.skins == null)
                continue;
            for (int j = 0; j < tower.SkinData.skins.Count; j++)
            {
                var skin = tower.SkinData.skins[j];
                if (towerStateMap == null)
                {
                    Debug.Log("towerStateMap == null");
                }

                if (!towerStateMap.TryGetValue(tower.ID, out var test_state))
                {
                    Debug.Log($"towerStateMap 不包含 towerID={tower.ID}");
                }

                if (test_state == null)
                {
                    Debug.Log("ItemState == null");
                }

                if (test_state.AllSkins == null)
                {
                    Debug.Log("AllSkins == null");
                }
                Debug.Log($"AllSkins.Count = {test_state.AllSkins.Count}");
                if (skin == null || skin.ID == 0)
                    continue; // ID=0 默认皮肤不参与

                float weight = 1f;
                // 如果已经解锁 → 权重减半
                if (towerStateMap.TryGetValue(tower.ID, out var state) &&
                    state.AllSkins != null &&
                    state.AllSkins.TryGetValue(skin.ID, out var skinState) &&
                    skinState.IsUnlocked)
                {
                    weight *= 0.5f;
                }

                pool.Add((tower, skin, weight));
                totalWeight += weight;
            }
        }

        // 安全兜底
        if (pool.Count == 0 || totalWeight <= 0f)
        {
            Debug.LogError("GetTowerSkinPiece: 没有可抽取的皮肤");
            return (null, null);
        }

        // 权重随机
        float rand = UnityEngine.Random.Range(0f, totalWeight);

        for (int i = 0; i < pool.Count; i++)
        {
            if (rand < pool[i].weight)
            {
                return (pool[i].tower, pool[i].skin);
            }

            rand -= pool[i].weight;
        }

        // 理论不会到这里，兜底返回最后一个
        var last = pool[pool.Count - 1];
        return (last.tower, last.skin);
    }


    /// 抽取一个炮塔底座皮肤碎片，返回 (TowerPlatformData, Skin)
    private (TowerPlatformData, Skin) GetTowerBaseSkinPiece()
    {
        float totalWeight = 0f;

        // 临时池，不用 Dictionary
        List<(TowerPlatformData tower, Skin skin, float weight)> pool
            = new List<(TowerPlatformData, Skin, float)>();

        var towerDatas = TowerPlatformDataManager.Instance.TowerPlatformDatas;
        var towerStateMap = DataManager.Instance.PlayerInfo.TowerPlatformStateMap;
        
        for (int i = 0; i < towerDatas.Count; i++)
        {
            var tower = towerDatas[i];
            if (tower == null || tower.SkinData == null || tower.SkinData.skins == null)
                continue;
            for (int j = 0; j < tower.SkinData.skins.Count; j++)
            {
                var skin = tower.SkinData.skins[j];
                if (towerStateMap == null)
                {
                    Debug.Log("TowerPlatformStateMap == null");
                }

                if (!towerStateMap.TryGetValue(tower.ID, out var test_state))
                {
                    Debug.Log($"TowerPlatformStateMap 不包含 towerID={tower.ID}");
                }

                if (test_state == null)
                {
                    Debug.Log("ItemState == null");
                }

                if (test_state.AllSkins == null)
                {
                    Debug.Log("AllSkins == null");
                }
                Debug.Log($"AllSkins.Count = {test_state.AllSkins.Count}");
                if (skin == null || skin.ID == 0)
                    continue; // 默认皮肤不参与

                float weight = 1f;
                
                // 已解锁 → 权重减半
                if (towerStateMap.TryGetValue(tower.ID, out var state) &&
                    state.AllSkins != null &&
                    state.AllSkins.TryGetValue(skin.ID, out var skinState) &&
                    skinState.IsUnlocked)
                {
                    weight *= 0.5f;
                }

                pool.Add((tower, skin, weight));
                totalWeight += weight;
            }
        }

        // 兜底
        if (pool.Count == 0 || totalWeight <= 0f)
        {
            Debug.LogError("GetTowerBaseSkinPiece: 没有可抽取的炮塔底座皮肤");
            return (null, null);
        }

        // 权重随机
        float rand = UnityEngine.Random.Range(0f, totalWeight);

        for (int i = 0; i < pool.Count; i++)
        {
            if (rand < pool[i].weight)
            {
                return (pool[i].tower, pool[i].skin);
            }
            rand -= pool[i].weight;
        }

        // 理论不会到这里，兜底
        var last = pool[pool.Count - 1];
        return (last.tower, last.skin);
    }
}
