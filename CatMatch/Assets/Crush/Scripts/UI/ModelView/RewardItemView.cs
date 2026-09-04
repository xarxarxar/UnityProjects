using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardItemView : MonoBehaviour
{
    public Image icon;
    public Text amount;
    public Image medalImg;
    private Vector2 originSize;

    public void SetUp(RewardModel rewardModel)
    {
        amount.text = MergeBeast.Utils.FormatNumber(rewardModel.amount);
        if (rewardModel.rewardType <= RewardType.Sorrow)
            icon.sprite = BeastPrefs.Instance.GetBeastPref((BeastId)rewardModel.rewardType).GetComponent<BeastBase>().icons[0];
        else
        {
            var iconTmp = GameData.Instance.rewardSprites.GetRewardSprite(rewardModel.rewardType);
            if (iconTmp != null)
            {
                icon.sprite = iconTmp;
            }
        }
        if (originSize == default)
            originSize = icon.rectTransform.sizeDelta;
        icon.SetNativeSize();
        var newSize = icon.rectTransform.sizeDelta;

        var scaleX = originSize.x / newSize.x;
        var scaleY = originSize.y / newSize.y;

        var sc = icon.rectTransform.localScale;
        sc.x = scaleX > scaleY ? scaleY : scaleX;
        sc.y = scaleX > scaleY ? scaleY : scaleX;
        icon.rectTransform.localScale = sc;

        if (rewardModel.rewardType >= RewardType.Atlantus && rewardModel.rewardType <= RewardType.Sorrow)
            medalImg.gameObject.SetActive(true);
        else medalImg.gameObject.SetActive(false);

        if (rewardModel.rewardType == RewardType.CommonMedalMonster)
        {
            amount.text = "普通";
        }
        else if (rewardModel.rewardType == RewardType.RandomMedalMonster)
        {
            amount.text = "随机";
        }
    }
}
