using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RewardCardType
{
    coinCard,
    scoreCard
}

public class RewardCard : Card
{
    [SerializeField] private Image coinIcon;
    [SerializeField] private Image scoreIcon;

    [SerializeField] private Text countText;

    private string tipString;
    private int count;

    Color32 coinColor = new Color32(253, 151, 68, 255);
    Color32 scoreColor = new Color32(28, 176, 246, 255);

    private void OnEnable()
    {
        
    }

    public void OnSet(RewardCardType rewardCardType,int count)
    {
        this.count = count;
        if (rewardCardType is RewardCardType.coinCard)
        {
            SetCoinCard();
        }
        else if(rewardCardType is RewardCardType.scoreCard)
        { 
            SetScoreCard(); 
        }
    }

    /// <summary>
    /// 从牌堆抽中了这个牌之后做什么
    /// </summary>
    public void OnInit()
    {
        GameEntrance.instance.CoinCount += count;
        ShowTipManager.instance.ShowTip(tipString, count);
        transform.DOScale(0, 0.5f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            DeckManager.instance.cardPool.ReturnCard(this);
        });
    }


    public void SetCoinCard()
    {
        coinIcon.gameObject.SetActive(true);
        scoreIcon.gameObject.SetActive(false);

        countText.text=$"+{count}";
        countText.color = coinColor;
        tipString = "获得金币";
    }

    public void SetScoreCard()
    {
        coinIcon.gameObject.SetActive(false);
        scoreIcon.gameObject.SetActive(true);

        countText.text = $"+{count}";
        countText.color = scoreColor;
        tipString = "获得分数";
    }
}
