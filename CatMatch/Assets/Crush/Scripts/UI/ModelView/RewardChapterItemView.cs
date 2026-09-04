using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardChapterItemView : MonoBehaviour
{
    public Image rewardImg;
    public Text amountTxt;
    public Image medalImg;

    public void SetUp(RewardData chapterRewardData)
    {
        if (chapterRewardData.rewardType >= RewardType.Atlantus && chapterRewardData.rewardType <= RewardType.VulcanArcher)
        {
            medalImg.gameObject.SetActive(true);
        }
        else medalImg.gameObject.SetActive(false);

        var rewardSp = GameData.Instance.rewardSprites.GetRewardSprite(chapterRewardData.rewardType);
        if (rewardSp != null) rewardImg.sprite = rewardSp;
        amountTxt.text = MergeBeast.Utils.FormatNumber(chapterRewardData.amount);
    }
}
