using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StorePanel : MonoBehaviour
{
    public bool isInited=false;
    public List<GoodsItem> AllGoods=new List<GoodsItem>();//所有可以卖的商品选项

    [SerializeField] private Transform GoodsParent;//商品生成的父物体
    [SerializeField] private Button UpdateButton;//刷新商品的按钮
    [SerializeField] private Button UpdateButtonAds;//刷新商品的广告按钮
    [SerializeField] private Text UpdateText;//刷新商品的剩余时长

    private float crownWeight = 0.1f;
    private float blessWeight = 0.65f;
    private float pieceWeight = 0.25f;

    private Coroutine nextDayCoro = null;
    private long nextDaySeconds = 0;

    private void OnEnable()
    {
        if (isInited) return;

        if (!DataManager.Instance.PlayerInfo.Config.TryGetValue("UpdateStoreCount", out var value))
        {
            DataManager.Instance.PlayerInfo.SetConfig("UpdateStoreCount", 3); // 每天默认看广告免费3次
        }

        GetNineGoods();
        UpdateButton.onClick.RemoveAllListeners();
        UpdateButton.onClick.AddListener(UpdateStore);

        //StartCountNextDay();
    }

    private void OnDisable()
    {
        if(nextDayCoro != null)
        {
            StopCoroutine(nextDayCoro);
        }
    }


    //刷新商店
    private void UpdateStore()
    {
        if (MetaCurrencyManager.Instance.SpendMetaCoin(RewardType.Diamond, 100))
        {
            DestroyAllGoods();
            GetNineGoods();
        }
        else
        {
            TipManager.Instance.ShowTip("钻石不足");
        }
    }

    /// <summary>
    /// 抽取九个商品
    /// </summary>
    private void GetNineGoods()
    {
        if (AllGoods.Count <= 0) return;
        isInited=true;

        float totalWeight = crownWeight + blessWeight + pieceWeight;

        for (int i=0;i<9;i++)
        {
            float ran=Random.Range(0,totalWeight);
            Debug.Log($"随机数为{ran}");
            GoodsItem goodsItemIns;
            if (ran< crownWeight)//抽取皇冠
            {
                Debug.Log("抽取皇冠");
                goodsItemIns = Instantiate(AllGoods[2], GoodsParent);
                goodsItemIns.Init();
                continue;
            }
            ran -= crownWeight;
            if(ran< blessWeight)//抽取祝福
            {
                Debug.Log("抽取祝福");
                goodsItemIns = Instantiate(AllGoods[1], GoodsParent);
            }
            else
            {
                //抽取碎片
                Debug.Log("抽取碎片");
                goodsItemIns = Instantiate(AllGoods[0], GoodsParent);
            }
            goodsItemIns.Init();
        }
    }

    private void DestroyAllGoods()
    {
        for(int i= GoodsParent.childCount-1; i>=0; i--)
        {
            Destroy(GoodsParent.GetChild(i).gameObject);
        }
    }

    //开始明天的倒计时
    private void StartCountNextDay()
    {
        if (nextDayCoro != null)
        {
            StopCoroutine(nextDayCoro);
        }
        nextDaySeconds =DataManager.GetSecondsUntilNextBeijingMidnight();
        StartCoroutine(NextDayIe(() =>
        {

        }));
    }

    private IEnumerator NextDayIe(System.Action onFinish)
    {
        while (nextDaySeconds > 0)
        {
            UpdateText.text ="剩余时间: "+ FormatTime(nextDaySeconds);
            yield return new WaitForSeconds(1f);
            nextDaySeconds--;
        }

        // 最后一帧
        UpdateText.text = "剩余时间: 00:00:00";

        onFinish?.Invoke();
    }
    private string FormatTime(long totalSeconds)
    {
        long hours = totalSeconds / 3600;
        long minutes = (totalSeconds % 3600) / 60;
        long seconds = totalSeconds % 60;

        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
