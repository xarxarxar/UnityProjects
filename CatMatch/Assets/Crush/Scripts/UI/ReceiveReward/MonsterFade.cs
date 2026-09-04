using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using BigIntegerC = System.Numerics.BigInteger;

public class MonsterFade : MonoBehaviour
{
    public RectTransform rect;
    public SkeletonGraphic skeletonGraphic;
    public Image medalImg;
    public Text amountTxt;
    public Text amountTxt2;
    public RectTransform animPart;
    public Image img;

    public void SetUp(RewardModel rewardModel)
    {
        BeastId beastId = BeastId.Akwa;
        bool isAnim = false;
        if ((int)rewardModel.rewardType <= (int)RewardType.Sorrow)
        {
            beastId = (BeastId)((int)(rewardModel.rewardType));
            isAnim = true;
        }

        if (isAnim)
        {
            img.gameObject.SetActive(false);
            img.transform.parent.gameObject.SetActive(false);

            animPart.gameObject.SetActive(true);
            medalImg.gameObject.SetActive(true);

            var skeletonDataAsset = SkeletonDatas.Instance.skeletonBeastPair[beastId].skeletonDataAsset;
            SkeletonGraphic skG = skeletonGraphic;

            skG.Clear();
            skG.skeletonDataAsset = skeletonDataAsset;
            skG.initialSkinName = "default";
            skG.startingAnimation = "";
            skG.Initialize(true);
            skG.AnimationState.SetAnimation(0, "idle", true);
            skG.SetMaterialDirty();

            amountTxt.gameObject.SetActive(true);
            amountTxt.text = "×" + Helper.FormatNumber(rewardModel.amount);
            amountTxt2.gameObject.SetActive(false);
        }
        else
        {
            img.transform.parent.gameObject.SetActive(true);
            img.gameObject.SetActive(true);
            
            animPart.gameObject.SetActive(false);
            medalImg.gameObject.SetActive(false);

            img.sprite = GameData.Instance.rewardSprites.GetRewardSprite(rewardModel.rewardType);
            img.SetNativeSize();

            var scaleX = 250 / img.rectTransform.sizeDelta.x;
            var scaleY = 250 / img.rectTransform.sizeDelta.y;
            if (scaleX > scaleY)
                img.rectTransform.localScale = new Vector3(scaleY, scaleY, scaleY);
            else
                img.rectTransform.localScale = new Vector3(scaleX, scaleX, scaleX);

            amountTxt.gameObject.SetActive(false);
            amountTxt2.gameObject.SetActive(true);
            amountTxt2.text = "×" + Helper.FormatNumber(rewardModel.amount);
        }
    }
}
