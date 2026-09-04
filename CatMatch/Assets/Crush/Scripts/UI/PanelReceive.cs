using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using BigIntegerC = System.Numerics.BigInteger;


public class PanelReceive : MonoBehaviour
{
    [Header("Part 1")]
    public RectTransform top;
    public SkeletonGraphic skeletonGraphic;
    public Image medalImg;
    public Text medalAmountTxt;
    public RectTransform animPart;
    public Image img;

    [Header("Part 2")]
    public RectTransform top22;
    public SkeletonGraphic skeletonGraphic22;
    public Image medalImg22;
    public Text medalAmountTxt22;
    public RectTransform animPart22;
    public Image img22;

    [Header("Chung")]
    public Text headerTxt;

    public RectTransform namePart;
    public RectTransform rarityParent;
    public RectTransform namePart2;

    public Text beastNameTxt;
    public Text nameTxt2;

    public Image medalImg2;
    public Text medalAmountTxt2;
    public Text medalAmountTxt2_2;

    public Slider processSlider;
    public Text processValueTxt;

    public Image fill;
    public Sprite fillNotFull;
    public Sprite fillFull;

    List<RewardModel> rewardModels;

    int current;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        Init(null);
    }

    public void Init(List<RewardModel> models)
    {
        current = 0;

        if (models == null)
        {
            rewardModels = new List<RewardModel>(){
                new RewardModel()
                {
                    rewardType = RewardType.Chaos,
                    amount = 5
                },
                new RewardModel()
                {
                    rewardType = RewardType.Gem,
                    amount = 6
                }
            };
        }

        top.gameObject.SetActive(true);
        top22.gameObject.SetActive(false);

        SetUp();
    }

    public void SetUp()
    {
        var rewardModel = rewardModels[current];

        if ((int)rewardModel.rewardType <= (int)RewardType.Sorrow)
        {
            headerTxt.gameObject.SetActive(true);

            var beastId = (BeastId)((int)(rewardModel.rewardType));
            Debug.Log("beastId:" + beastId);

            beastNameTxt.text = Helper.WordFilt(beastId.ToString(), out int numSpace);

            if (numSpace >= 2)
            {
                namePart.anchoredPosition = new Vector3(-70, 54, 0);
                rarityParent.anchoredPosition = new Vector3(7, 0, 0);
            }
            else
            {
                namePart.anchoredPosition = new Vector3(-140, 54, 0);
                rarityParent.anchoredPosition = new Vector3(-60, 0, 0);
            }

            ChangeTop(true);
            InitSkeleton(beastId, rewardModel.amount);

            int needMedalStar = 0;
            var beastData = PlayerData.instance.GetBeastData(beastId);

            if (beastData != null)
            {
                needMedalStar = GetNeedMedalNextStar(beastData.curStar);
            }
            else
            {
                needMedalStar = GetNeedMedalNextStar(0);
            }

            if (rewardModel.amount >= needMedalStar)
                fill.sprite = fillFull;
            else fill.sprite = fillNotFull;

            processSlider.maxValue = needMedalStar;
            processSlider.value = (int)rewardModel.amount;

            processValueTxt.text = string.Format("{0}/{1}", rewardModel.amount, needMedalStar);
        }
        else
        {
            headerTxt.gameObject.SetActive(false);

            var rewardSp = GameData.Instance.rewardSprites.GetRewardSprite(rewardModel.rewardType);
            nameTxt2.text = Helper.WordFilt(rewardModel.rewardType.ToString(), out int numSpace);
            if (numSpace >= 2)
            {
                namePart2.anchoredPosition = new Vector3(-70, 54, 0);
            }
            else
            {
                namePart2.anchoredPosition = new Vector3(-140, 54, 0);
            }

            ChangeTop(false);
            InitImage(rewardModel);
        }

        medalAmountTxt2.text = "×" + rewardModel.amount.ToString();
        medalAmountTxt2_2.text = "×" + rewardModel.amount.ToString();
    }

    void ChangeTop(bool isAnim)
    {
        if (current % 2 == 0)
        {
            top.gameObject.SetActive(true);
            top.GetComponent<Canvas>().sortingLayerID = 103;
            top22.GetComponent<Canvas>().sortingLayerID = 102;

            if (isAnim)
            {
                img.gameObject.SetActive(false);
                animPart.gameObject.SetActive(true);
                medalImg.gameObject.SetActive(true);
            }
            else
            {
                img.gameObject.SetActive(true);
                animPart.gameObject.SetActive(false);
                medalImg.gameObject.SetActive(false);
            }
            anim.Play("Receive1In");
        }
        else
        {
            top22.gameObject.SetActive(true);
            top.GetComponent<Canvas>().sortingLayerID = 102;
            top22.GetComponent<Canvas>().sortingLayerID = 103;

            if (isAnim)
            {
                img22.gameObject.SetActive(false);
                animPart22.gameObject.SetActive(true);
                medalImg22.gameObject.SetActive(true);
            }
            else
            {
                img22.gameObject.SetActive(true);
                animPart22.gameObject.SetActive(false);
                medalImg22.gameObject.SetActive(false);
            }

            anim.Play("Receive2In");
        }

        if (!isAnim)
        {
            rarityParent.gameObject.SetActive(false);
            processSlider.gameObject.SetActive(false);
            medalImg2.gameObject.SetActive(false);

            namePart.gameObject.SetActive(false);
            namePart2.gameObject.SetActive(true);
        }
        else
        {
            rarityParent.gameObject.SetActive(true);
            processSlider.gameObject.SetActive(true);
            medalImg2.gameObject.SetActive(true);

            namePart.gameObject.SetActive(true);
            namePart2.gameObject.SetActive(false);
        }
    }

    void InitSkeleton(BeastId beastId, BigIntegerC rewardAmount)
    {
        var skeletonDataAsset = SkeletonDatas.Instance.skeletonBeastPair[beastId].skeletonDataAsset;

        SkeletonGraphic skG = null;
        Text txt = null;
        if (current % 2 == 0)
        {
            skG = skeletonGraphic;
            txt = medalAmountTxt;
        }
        else
        {
            skG = skeletonGraphic22;
            txt = medalAmountTxt22;
        }

        skG.Clear();
        skG.skeletonDataAsset = skeletonDataAsset;
        skG.initialSkinName = "default";
        skG.startingAnimation = "";
        skG.Initialize(true);
        skG.AnimationState.SetAnimation(0, "idle", true);
        skG.SetMaterialDirty();

        txt.text = "×" + rewardAmount.ToString();
    }

    void InitImage(RewardModel rewardModel)
    {
        Image imgTmp = null;
        Text txt = null;
        if (current % 2 == 0)
        {
            imgTmp = img;
            txt = medalAmountTxt;
        }
        else
        {
            imgTmp = img22;
            txt = medalAmountTxt22;
        }
        imgTmp.sprite = GameData.Instance.rewardSprites.GetRewardSprite(rewardModel.rewardType);
        imgTmp.SetNativeSize();

        var scaleX = 250 / imgTmp.rectTransform.sizeDelta.x;
        var scaleY = 250 / imgTmp.rectTransform.sizeDelta.y;
        if (scaleX > scaleY)
            imgTmp.rectTransform.localScale = new Vector3(scaleY, scaleY, scaleY);
        else
            imgTmp.rectTransform.localScale = new Vector3(scaleX, scaleX, scaleX);

        txt.text = "×" + rewardModel.amount.ToString();
    }

    public void OnClick()
    {
        if (current < rewardModels.Count - 1)
        {
            current++;
            SetUp();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    int GetNeedMedalNextStar(int star)
    {
        if (star == 5) return 10_000;

        for (int k = Constant.medalByStars.Count - 1; k >= 0; k--)
        {
            if (star >= k)
            {
                return Constant.medalByStars[k];
            }
        }
        return 10_000;
    }

    public void Top1In()
    {
        top22.gameObject.SetActive(false);
    }

    public void Top1Out()
    {
    }

    public void Top2Out()
    {
    }

    public void Top2In()
    {
        top.gameObject.SetActive(false);
    }

    public void Attack()
    {
        Debug.Log("Attackkkkkkkk");
        skeletonGraphic.AnimationState.ClearTracks();
        skeletonGraphic.AnimationState.SetAnimation(0, "skill01_full", true);
    }

    public void Idle()
    {
        Debug.Log("Idleeeeeeeeeee");
        skeletonGraphic.AnimationState.ClearTracks();
        skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
    }
}
