using System.Collections;
using System.Collections.Generic;
using Observer;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using BigIntegerC = System.Numerics.BigInteger;


public class PanelReceive2 : MonoBehaviour
{
    public List<MonsterFade> monsterFades = new List<MonsterFade>(2);

    [Header("Chung")]
    public Text headerTxt;

    public RectTransform rarityParent;

    public RectTransform namePart;
    public Text nameTxt;
    public Image medalImg;
    public Text amountTxt;

    public RectTransform namePart2;
    public Text nameTxt2;
    public Text amountTxt2;

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
        // Init(null);
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
        else rewardModels = models;

        Debug.Log("rewardModels:" + rewardModels.Count);

        monsterFades[0].gameObject.SetActive(true);
        monsterFades[1].gameObject.SetActive(false);

        SetUp();
    }

    public void SetUp()
    {
        var rewardModel = rewardModels[current];

        ChangeTop(rewardModel);

        if ((int)rewardModel.rewardType <= (int)RewardType.Sorrow)
        {
            headerTxt.gameObject.SetActive(true);

            var beastId = (BeastId)((int)(rewardModel.rewardType));
            nameTxt.text = Helper.WordFilt(beastId.ToString(), out int numSpace);

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
            amountTxt.text = "×" + rewardModel.amount.ToString();

            // slider part
            int needMedalStar = 0;
            var beastData = PlayerData.instance.GetBeastData(beastId);
            if (beastData != null) needMedalStar = GetNeedMedalNextStar(beastData.curStar);
            else needMedalStar = GetNeedMedalNextStar(0);

            if (rewardModel.amount >= needMedalStar)
                fill.sprite = fillFull;
            else fill.sprite = fillNotFull;

            if (beastData.curStar != 5)
            {
                processSlider.maxValue = needMedalStar;
                processSlider.value = (int)beastData.curMedal;
                processValueTxt.text = string.Format("{0}/{1}", beastData.curMedal, needMedalStar);
            }
            else
            {
                processSlider.maxValue = 1;
                processSlider.value = 0;
                processValueTxt.text = string.Format("{0}/{1}", beastData.curMedal, "...");
            }
        }
        else
        {
            headerTxt.gameObject.SetActive(false);

            nameTxt2.text = Helper.WordFilt(rewardModel.rewardType.ToString(), out int numSpace);

            if (numSpace >= 2) namePart2.anchoredPosition = new Vector3(-70, 54, 0);
            else namePart2.anchoredPosition = new Vector3(-140, 54, 0);

            amountTxt2.text = "×" + rewardModel.amount.ToString();
        }
    }

    void ChangeTop(RewardModel rewardModel)
    {
        var index = current % 2;
        monsterFades[index].gameObject.SetActive(true);
        monsterFades[index].SetUp(rewardModel);

        for (int k = 0; k < 2; k++)
        {
            monsterFades[k].gameObject.GetComponent<Canvas>().sortingLayerID = (index == k) ? 103 : 102;
        }
        if (index == 0) anim.Play("Receive1In");
        else if (index == 1) anim.Play("Receive2In");

        if ((int)rewardModel.rewardType > (int)RewardType.Sorrow)
        {
            rarityParent.gameObject.SetActive(false);
            processSlider.gameObject.SetActive(false);
            medalImg.gameObject.SetActive(false);

            namePart.gameObject.SetActive(false);
            namePart2.gameObject.SetActive(true);
        }
        else
        {
            rarityParent.gameObject.SetActive(true);
            processSlider.gameObject.SetActive(true);
            medalImg.gameObject.SetActive(true);

            namePart.gameObject.SetActive(true);
            namePart2.gameObject.SetActive(false);
        }
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
            this.PostEvent(EventID.EndOnClickReceiveShopTut);
        }
        this.PostEvent(EventID.OnClickReceiveShopTut);
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
        monsterFades[1].gameObject.SetActive(false);
    }

    public void Top1Out()
    {
    }

    public void Top2Out()
    {
    }

    public void Top2In()
    {
        monsterFades[0].gameObject.SetActive(false);
    }
}
