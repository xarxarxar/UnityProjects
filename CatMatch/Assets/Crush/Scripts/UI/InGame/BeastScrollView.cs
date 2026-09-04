using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BeastScrollView : MonoBehaviour
{
    public Transform contentPanel;

    // private Dictionary<BeastId, BeastInfoItem> beastInfoItemPair;

    [SerializeField] private GameObject beastItemViewPref;

    void Start()
    {
        // beastInfoItemPair = new Dictionary<BeastId, BeastInfoItem>();
    }

    public void Clear()
    {
        // beastInfoItemPair.Clear();

        RemoveItemViews();
    }

    private void RemoveItemViews()
    {
        while (contentPanel.childCount > 0)
        {
            var go = transform.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }
    }

    public void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems)
    {
        Sort(beastInfoItems);
    }

    // public void UpdateBeastInfo(BeastId id, BeastInfoItem item)
    // {
    //     if (!beastInfoItemPair.ContainsKey(id)) return;

    //     beastInfoItemPair[id].damage = item.damage;
    //     // beastInfoItems[id].icon = item.damage;
    //     beastInfoItemPair[id].beastIndex = item.beastIndex;
    //     beastInfoItemPair[id].beastName = item.beastName;

    //     Sort();
    // }

    public void Sort(List<BeastInfoItem> beastInfoItems)
    {
        RemoveItemViews();

        var _beastInfoItems = beastInfoItems.Where(bi=>bi.curTeam == Team.My);
        if (_beastInfoItems == null || _beastInfoItems.Count() <= 0) return;

        var datas = _beastInfoItems.OrderByDescending(bif => bif.damage);

        var dataFirst = datas.First();

        int dmgMax = 0;

        if (dataFirst != null) dmgMax = dataFirst.damage;

        for (int k = 0; k < datas.Count(); k++)
        {
            var itemData = datas.ElementAt(k);
            
            itemData.showDamage = true;

            var go = ObjectPool.Instance.GetGameObject(beastItemViewPref, Vector3.zero, Quaternion.identity);

            go.transform.SetParent(transform);

            go.transform.localScale = new Vector3(1, 1, 1);

            go.GetComponent<BeastInfoItemView>().SetUp(itemData, dmgMax);
        }
    }
}
