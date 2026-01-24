using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CollectionPanel : BasePanel
{
    public CollectionText CollectionPrefab;//收集的文字的prefab
    public Transform[] Collections;//四行父物体
    public Button exchangeButton;//兑换成奖牌的按钮
    public List<CollectionText> allCollections=new List<CollectionText>();

    private int Medal => DataManager.Instance.PlayerInfo.Medal;
    /// <summary>
    /// 这个是初始化面板的UI状态，每次打开面板的时候用
    /// </summary>
    protected override void InitPanel()
    {
        allCollections.Clear();
        exchangeButton.onClick.RemoveAllListeners();
        exchangeButton.onClick.AddListener(Exchange);

        string chars = CollectionManager.Chars;
        int charsPerRow = 5; // 每行5个，共4行 = 20个

        // 清理旧UI（避免面板再次打开时重复生成）
        foreach (var parent in Collections)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        // 逐个生成 prefab
        for (int i = 0; i < chars.Length; i++)
        {
            int row = i / charsPerRow;   // 第几行

            if (row >= Collections.Length)
                break;

            Transform parent = Collections[row];

            // 实例化 prefab
            CollectionText ct = Instantiate(CollectionPrefab, parent);

            char c = chars[i];
            Color32 color = CollectionManager.Instance.GetColorByIndex(i);
            // 设置显示字符 + 颜色
            if (!DataManager.Instance.PlayerInfo.MonthlyCollection.ContainsKey(i)) DataManager.Instance.PlayerInfo.MonthlyCollection[i] = 0;
            ct.SetCollectionText(c, color, DataManager.Instance.PlayerInfo.MonthlyCollection[i]);
            allCollections.Add(ct);
        }
    }

    private void Exchange()
    {
        //都至少有1个
        bool allGreaterThanZero = DataManager.Instance.PlayerInfo.MonthlyCollection
    .All(kv => kv.Value > 0);

        if (!allGreaterThanZero)
        {
            TipManager.Instance.ShowTip("尚未集齐所有内容");
            return;
        }
        for(int i = 0;i< allCollections.Count;i++)
        {
            UIUtils.PlayNumberAnimation(allCollections[i].Count, DataManager.Instance.PlayerInfo.MonthlyCollection[i]-1, 0.5f);
            allCollections[i].SetCollectionText(CollectionManager.Chars[i], CollectionManager.Instance.GetColorByIndex(i), DataManager.Instance.PlayerInfo.MonthlyCollection[i] - 1);
        }
        // 先缓存要修改的键
        List<int> keys = new List<int>();

        foreach (var kv in DataManager.Instance.PlayerInfo.MonthlyCollection)
        {
            keys.Add(kv.Key);
        }

        // 遍历 keys 来修改原字典
        foreach (var key in keys)
        {
            int oldVaule = DataManager.Instance.PlayerInfo.MonthlyCollection[key];
            DataManager.Instance.PlayerInfo.SetMonthlyCollection(key,oldVaule-1);
        }

        DataManager.Instance.PlayerInfo.SetMedal(Medal+1);
        GameUIManager.Instance.ShowGetRewardPanel((RewardType.Medal, 1));
        AudioManager.Instance.PlaySFX("领取奖励");
    }
}
