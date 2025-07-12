using SuperScrollView;
using UnityEngine;
using UnityEngine.UI;

public class DrawBlessPanel : BasePanel
{
    [SerializeField]private LoopListView2 _loopListView;
    [SerializeField] private Button _drawButton;//抽奖按钮
    [SerializeField] private RewardStruct _cost;//抽奖所需的花费

    public override void OnEnable()
    {
        base.OnEnable();
        if (BlessManager.Instance == null)
        {
            Debug.Log("BlessManager.Instance == null");
        }
        _cost.Init(BlessManager.Instance.BlessCost);//花费

        _drawButton.onClick.AddListener(DrawBless);
        //拥有的钻石数量要大于等于花费的数量
        _drawButton.interactable=MetaCurrencyManager.Instance.DiamondCount.Value>= BlessManager.Instance.BlessCost;
        _cost.countText.color = MetaCurrencyManager.Instance.DiamondCount.Value >= BlessManager.Instance.BlessCost ? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255);
    }

    private void OnDisable()
    {
        _drawButton.onClick.RemoveAllListeners();
    }

    //抽取祝福
    private void DrawBless()
    {
        Debug.Log($"抽中的数字为{BlessManager.Instance.GetRandomBless().testProp}");

        //消耗钻石
        MetaCurrencyManager.Instance.SpendMetaCoin(RewardType.Diamond, BlessManager.Instance.BlessCost);
        //更新需要花费的钻石数量
        BlessManager.Instance.BlessCost += 100;//每一次需要花费的钱都加100
        //更新按钮UI
        _drawButton.interactable = MetaCurrencyManager.Instance.DiamondCount.Value >= BlessManager.Instance.BlessCost;
        _cost.countText.color = MetaCurrencyManager.Instance.DiamondCount.Value >= BlessManager.Instance.BlessCost ? new Color32(239, 241, 245, 255) : new Color32(228, 73, 98, 255); 
        _cost.Init(BlessManager.Instance.BlessCost);//花费
    }
}
