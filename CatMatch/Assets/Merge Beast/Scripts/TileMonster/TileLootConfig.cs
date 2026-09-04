using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MergeBeast;

namespace Tiledom {
    [CreateAssetMenu(fileName = "TileLootConfig", menuName = "ConfigData/TileLootConfig")]
    public class TileLootConfig : ScriptableObject {
        public List<ListLootItemData> list;

        public ListLootItemData GetLevelReward(int level) {
            return list[level - 1];
        }

        public int GetAmountBuyType(int level, int type) {
            ListLootItemData levelReward = GetLevelReward(level);
            for(int i = 0; i < levelReward.listItem.Count; i++) {
                LootItemData itemData = levelReward.listItem[i];
                if(itemData.lootType == (EnumDefine.TileLoot)type) {
                    return itemData.amount;
                }
            }
            return 1;
        }
    }

    [System.Serializable]
    public class ListLootItemData {
        public List<LootItemData> listItem;
    }
    [System.Serializable]
    public class LootItemData {
        public EnumDefine.TileLoot lootType;
        public int amount;
    }
}