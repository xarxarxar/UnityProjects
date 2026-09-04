using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScrollViewStats : MonoBehaviour
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

    public void UpdateBeastInfoItems(List<BeastInfoItem> beastInfoItems, bool showDamage)
    {
        Sort(beastInfoItems, showDamage);
    }

    private void Sort(List<BeastInfoItem> beastInfoItems, bool showDamage)
    {
        RemoveItemViews();

        var _beastInfoItems = beastInfoItems.Where(bi => bi.curTeam == Team.Enemy);

        if (_beastInfoItems == null || _beastInfoItems.Count() <= 0) return;

        var datas = _beastInfoItems.OrderByDescending(bif => showDamage? bif.damage: bif.damaged);

        var dataFirst = datas.First();


        int dmgMax = 0;

        if (dataFirst != null) dmgMax = showDamage ? dataFirst.damage : dataFirst.damaged;
        
        // Debug.Log("enemy showDamage:" + showDamage + " dmgMax:" + dmgMax);

        for (int k = 0; k < datas.Count(); k++)
        {
            var itemData = datas.ElementAt(k);

            itemData.showDamage = showDamage;

            var go = ObjectPool.Instance.GetGameObject(pref, Vector3.zero, Quaternion.identity);

            go.transform.SetParent(transform);

            go.transform.localScale = new Vector3(1, 1, 1);

            go.GetComponent<BeastInfoItemView>().SetUp(itemData, dmgMax);
        }
    }
}
