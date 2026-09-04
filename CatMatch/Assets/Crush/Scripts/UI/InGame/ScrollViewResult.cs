using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScrollViewResult : MonoBehaviour
{
    public Transform contentPanel;

    [SerializeField] private GameObject pref;

    private void RemoveItemViews()
    {
        while (contentPanel.childCount > 0)
        {
            var go = transform.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }
    }

    public void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems, bool isWin)
    {
        Sort(beastInfoItems, isWin);
    }

    public void Sort(List<BeastInfoItem> beastInfoItems, bool isWin)
    {
        RemoveItemViews();

        var _beastInfoItems = beastInfoItems.Where(bi => bi.curTeam == Team.My);
        if (_beastInfoItems == null || _beastInfoItems.Count() <= 0) return;

        var datas = _beastInfoItems.OrderByDescending(bif => bif.damage);

        var dataFirst = datas.First();

        int dmgMax = 0;

        if (dataFirst != null) dmgMax = dataFirst.damage;

        for (int k = 0; k < datas.Count(); k++)
        {
            if (isWin && k >= 3) return;
            if (!isWin && k >= 5) return;

            var itemData = datas.ElementAt(k);

            var go = ObjectPool.Instance.GetGameObject(pref, Vector3.zero, Quaternion.identity);

            go.transform.SetParent(transform);

            go.transform.localScale = new Vector3(1, 1, 1);

            go.GetComponent<BeastInfoItemView>().SetUp(itemData, dmgMax);
        }
    }
}
