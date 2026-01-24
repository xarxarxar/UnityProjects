using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlessPanel : BasePanel
{
    private bool isInited=false;//是否已经初始化
    [SerializeField] private Button _giveupButton;

    [System.Serializable]
    class BlessShow
    {
        public Text NameText;
        public Text DescriptionText;
        public Text CostText;
        public Text RemainText;//仓库剩余几个
        public Image IconImage;//图标
        public Image BackgroundImage;//背景图片
        public Image CircleImage;//圆圈的图片
        public Image NameBackgroundImage;//名称背景的图片
        public Button BlessButton;
    }

    [SerializeField]private BlessShow leftBlessShow;
    [SerializeField] private BlessShow rightBlessShow;

    private int leftCost = 0;
    private int rightCost = 0;

    private Color32 enoughCostColor= new Color32(215, 215, 215, 255);//金币足够时的颜色
    private Color32 notEnoughCostColor = new Color32(228, 73, 98, 255);//金币不足时的颜色

    /// <summary>
    /// 这个是初始化面板的UI状态，每次打开面板的时候用
    /// </summary>
    protected override void InitPanel()
    {
        BattleManager.Instance.PauseGame();//暂停游戏
        if (isInited) return;
        isInited=true;
        _giveupButton.interactable = true;
        _giveupButton.onClick.AddListener(() =>
        {
            OnFinish();
        });

        List<(Bless, int)> BlessAndRarity = BlessDataManager.Instance.ChooseBless(2);
        
        Bless LeftBless = BlessAndRarity[0].Item1;
        int LeftRarity= BlessAndRarity[0].Item2;
        leftCost = LeftBless.Costs[LeftRarity];
        SetBlessShow(leftBlessShow, LeftBless, LeftRarity);

        Bless RightBless = BlessAndRarity[1].Item1;
        int RightRarity = BlessAndRarity[1].Item2;
        rightCost= RightBless.Costs[RightRarity];
        SetBlessShow(rightBlessShow, RightBless, RightRarity);
        CurrencyManager.OnCoinChange -= OnCoinChange;
        CurrencyManager.OnCoinChange += OnCoinChange;
    }

    public override void OnCloseButton()
    {
        BattleManager.Instance.ResumeGame();//恢复游戏
        base.OnCloseButton();
    }

    //设置SetBlessShow字段
    private void SetBlessShow(BlessShow blessShow,Bless bless,int rarity)
    {
        blessShow.NameText.text = bless.BlessName;
        blessShow.DescriptionText.text = bless.Descriptions[rarity];
        blessShow.CostText.text = bless.Costs[rarity].ToString();
        if (!DataManager.Instance.PlayerInfo.BlessCount.TryGetValue(bless.ID, out var counts))
        {
            DataManager.Instance.PlayerInfo.SetBlessCount(bless.ID, new List<int> { -1, 10, 0 });
            counts = DataManager.Instance.PlayerInfo.BlessCount[bless.ID];
        }
        if(rarity == 0)
        {
            blessShow.RemainText.text = $"仓库剩余无限个";
        }
        else
        {
            blessShow.RemainText.text = $"仓库剩余{counts[rarity]}个";
        }
        

        int cost = bless.Costs[rarity];
        bool isEnough = CurrencyManager.Instance.HasEnoughGold(cost);
        //根据金币数量设置颜色
        if (isEnough)
        {
            blessShow.CostText.color = enoughCostColor;
        }
        else 
        {
            blessShow.CostText.color = notEnoughCostColor;
        }
        blessShow.IconImage.sprite = bless.sprite;
        if (rarity == 0)
        {
            blessShow.BackgroundImage.color = BlessDataManager.Instance.normalColor;
            blessShow.CircleImage.color = BlessDataManager.Instance.normalColor;
            blessShow.NameBackgroundImage.color = BlessDataManager.Instance.normalColor;
            blessShow.RemainText.color = BlessDataManager.Instance.normalColor;
        }
        else if(rarity == 1)
        {
            blessShow.BackgroundImage.color = BlessDataManager.Instance.rareColor;
            blessShow.CircleImage.color = BlessDataManager.Instance.rareColor;
            blessShow.NameBackgroundImage.color = BlessDataManager.Instance.rareColor;
            blessShow.RemainText.color = BlessDataManager.Instance.rareColor;
        }
        else if (rarity == 2)
        {
            blessShow.BackgroundImage.color = BlessDataManager.Instance.epicColor;
            blessShow.CircleImage.color = BlessDataManager.Instance.epicColor;
            blessShow.NameBackgroundImage.color = BlessDataManager.Instance.epicColor;
            blessShow.RemainText.color = BlessDataManager.Instance.epicColor;
        }
        
        //设置按钮
        blessShow.BlessButton.interactable = true;
        blessShow.BlessButton.onClick.RemoveAllListeners();
        blessShow.BlessButton.onClick.AddListener(() =>
        {
            BuyBless(cost, () =>
            {
                BlessInstance blessInstance= bless.CreateInstance(rarity);
                BlessManager.Instance.AddBlessInstance(blessInstance);//添加进当前的祝福列表、
                Debug.Log($"添加祝福");
                blessInstance.Start();
            });
            
        });
    }
    //购买
    private void BuyBless(int cost, UnityAction callback)
    {
        //购买成功
        if(CurrencyManager.Instance.SpendCoin(cost)) 
        {
            OnFinish();
            callback?.Invoke();//执行祝福
        }
        else
        {
            TipManager.Instance.ShowTip("金币不足");
        }
    }

    private void OnFinish()
    {
        isInited = false;
        //关闭按钮的可交互状态
        leftBlessShow.BlessButton.interactable = false;
        rightBlessShow.BlessButton.interactable = false;
        _giveupButton.interactable = false;
        BlessManager.Instance.UpgradeStage();//祝福的结算提升，最大祝福值提升

        //BattleManager.Instance.ResumeGame();//恢复游戏
        OnCloseButton();
    }

    
    private void OnCoinChange(int coin)
    {
        if (CurrencyManager.Instance.Gold >= leftCost)
        {
            leftBlessShow.CostText.color = enoughCostColor;
        }
        else
        {
            leftBlessShow.CostText.color = notEnoughCostColor;
        }

        if(CurrencyManager.Instance.Gold >= rightCost)
        {
            rightBlessShow.CostText.color = enoughCostColor;
        }
        else
        {
            rightBlessShow.CostText.color = notEnoughCostColor;
        }
    }
}
