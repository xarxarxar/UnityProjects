using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.EventSystems;
using MergeBeast;
using Observer;

[System.Serializable]
public class ElementSprite
{
    public Element element;
    public Sprite iconRound;
    public Sprite bg;
    public string efficacy1;
    public string efficacy2;
}

[System.Serializable]
public class BeastSkeletonGraphic
{
    public BeastId beastId;
    public SkeletonGraphic skeletonGraphic;
}

public enum EnableType
{
    FromListMonster,
    FromListEditTeam,
    InTeamEditTeam
}

public class BeastInfoHomePanel : MonoBehaviour, IMonsterAtributeUpDown
{
    public static BeastInfoHomePanel Instance;

    public Text beastNameTxt;
    public MonsterAtributeView elementImg;
    public MonsterAtributeView classImg;
    public List<ElementSprite> elementSprites;
    // public List<BeastClassSprite> beastClassSprites;

    public List<Transform> contents;

    //content1
    public List<Image> stars;
    public Sprite starOn;
    public Sprite starOff;
    public Image classRoundImg;
    public Image beastBgImg;
    public List<BeastSkeletonGraphic> beastSkeletonGraphics;

    public List<MonsterAtributeView> abilityImgs;
    public List<MonsterAtributeView> typeImgs;//sex, fly/walk

    public Text damageValueTxt;
    public Text hpValueTxt;
    public Text defValueTxt;
    public Image elementImg2;
    public Text classTxt;
    public TextMeshProUGUI affinity1Txt;
    public TextMeshProUGUI affinity2Txt;
    public Text levelTxt;

    public Text curExpTxt;
    public Text maxExpTxt;
    public Text curMedalTxt;
    public Text maxMedalTxt;
    public Image levelUpImg;
    public Image joinTeamImg;
    public Image evolveImg;
    public Text evolveTxt;
    public Sprite evolveDisableSp;
    public Sprite evolveEnableSp;

    public RectTransform toolTip;
    public RectTransform abilityToolTip;

    public Button prevButton;
    public Button nextButton;

    [Header("Content2")]
    public List<StatInfoItemView> statInfoItemViews;
    public List<SkillHomeInfoItemView> skillHomeInfoItemViews;

    Transform parentTeam, subParent;

    private BeastTeamInfo beastTeamInfo;
    private List<BeastTeamInfo> beastTeamInfos;
    private BeastBase beastBase;

    private int currentBeastIndex;
    private int currentContent;
    private int currentStatIndex;
    private System.Numerics.BigInteger nextExp;
    private System.Numerics.BigInteger nextMedal;

    private EnableType enableType;

    void Awake()
    {
        Instance = this;

        var btn = levelUpImg.GetComponent<Button>();
        btn.onClick.AddListener(() => LevelUp());

        btn = evolveImg.GetComponent<Button>();
        btn.onClick.AddListener(() => Evolve());
        UnityEngine.UI.ColorBlock colors = btn.colors;
        Color disabledColor = colors.disabledColor;
        disabledColor.a = 1;
        colors.disabledColor = disabledColor;
        btn.colors = colors;
    }

    void SwitchContent()
    {
        for (int k = 0; k < contents.Count; k++)
        {
            contents[k].gameObject.SetActive(currentContent == k);
        }
    }

    void OnEnable()
    {
        // AdsManager.Instance?.HideBanner();
    }

    void OnDisable()
    {
        // if (this.enableType == EnableType.FromListMonster)
        // {
        //     AdsManager.Instance?.RequestBanner();
        // }
    }

    public void Init(Transform _parentTeam, BeastTeamInfo beastHomeInfo, List<BeastTeamInfo> datas, EnableType enableType)
    {
        beastTeamInfos = datas;
        beastTeamInfo = beastHomeInfo;
        parentTeam = _parentTeam;

        nextButton.interactable = datas.Count != 1;
        prevButton.interactable = datas.Count != 1;

        Debug.Log(" prevButton.interactable:" + prevButton.interactable);

        if (joinTeamImg == null)
            Debug.Log("WTFFFFFFFFFFFFFF");

        this.enableType = enableType;
        if (enableType == EnableType.FromListMonster)
        {
            // levelUpImg.gameObject.SetActive(false);
            levelUpImg.gameObject.SetActive(true);
            UpdateLevel();

            joinTeamImg.gameObject.SetActive(false);
        }
        else if (enableType == EnableType.FromListEditTeam)
        {
            // levelUpImg.gameObject.SetActive(false);

            var condition = beastHomeInfo.curStar > 0 && TeamBuildPanel.emptySlotAmout > 0;
            Debug.Log(_parentTeam.parent.parent.name + " " + TeamBuildPanel.emptySlotAmout + " condition:" + condition);

            joinTeamImg.gameObject.SetActive(condition);
            levelUpImg.gameObject.SetActive(!condition);
            if (!condition) UpdateLevel();
        }
        else if (enableType == EnableType.InTeamEditTeam)
        {
            levelUpImg.gameObject.SetActive(true);
            UpdateLevel();
            joinTeamImg?.gameObject.SetActive(false);
        }

        currentBeastIndex = datas.IndexOf(beastHomeInfo);
        currentContent = 0;
        currentStatIndex = 0;

        if (beastHomeInfo.curStar <= 0) evolveTxt.text = "召唤";
        else evolveTxt.text = "进化";

        SwitchContent();
        if (beastTeamInfo == null) Debug.Log("wtfffffffff");
        if (beastNameTxt == null) Debug.Log("wtfffffffff 2");

        beastNameTxt.text = Helper.WordFilt(beastTeamInfo.beastId.ToString(), out int numSpace);
        var beastAllInfo = BeastPrefs.Instance.GetBeastAllInfo(beastTeamInfo.beastId);
        beastBase = beastAllInfo.pref.GetComponent<BeastBase>();

        // Debug.Log("Init beastId:" + beastBase.beastId + " atk:" + beastBase.atk + " hp:" + beastBase.hp);

        elementImg.SetUp(this, MonsterAtributeType.ElementType, element: beastBase.element);
        // classImg.sprite = beastBase.classSp;
        classImg.SetUp(this, MonsterAtributeType.ClassType, beastClass: beastBase.beastClass);

        var elementInfo = elementSprites.FirstOrDefault(e => e.element == beastBase.element);
        classRoundImg.sprite = elementInfo.iconRound;

        beastBgImg.sprite = elementInfo.bg;

        // typeImgs[0].sprite = beastBase.genderSp;
        // typeImgs[1].sprite = beastBase.moveSp;
        typeImgs[0].SetUp(this, MonsterAtributeType.GenderType, genderType: beastBase.genderType);
        typeImgs[1].SetUp(this, MonsterAtributeType.MoveType, moveType: beastBase.moveType);


        for (int k = 0; k < abilityImgs.Count; k++)
        {
            if (k < beastBase.abilityModels.Count)
            {
                // abilityImgs[k].sprite = beastBase.abilitySps[k];
                abilityImgs[k].SetUp(this, MonsterAtributeType.AbilityType, abilityType: beastBase.abilityModels[k].abilityType);
            }
            else
            {
                abilityImgs[k].gameObject.SetActive(false);
            }
        }

        affinity1Txt.text = string.IsNullOrEmpty(elementInfo.efficacy1) ? "" : elementInfo.efficacy1;
        affinity2Txt.text = string.IsNullOrEmpty(elementInfo.efficacy2) ? "" : elementInfo.efficacy2;

        for (int i = 0; i < beastSkeletonGraphics.Count; i++)
        {
            if (beastSkeletonGraphics[i].beastId == beastTeamInfo.beastId)
            {
                beastSkeletonGraphics[i].skeletonGraphic.gameObject.SetActive(true);
            }
            else
            {
                beastSkeletonGraphics[i].skeletonGraphic.gameObject.SetActive(false);
            }
        }

        elementImg2.sprite = elementImg.image.sprite;
        classTxt.text = beastBase.element + " 效果";

        UpdateBaseAtribute();

        UpdateLevel();
        UpdateStars();

        UpdateContent2();
    }

    public void SetSubParent(Transform subParent)
    {
        this.subParent = subParent;
    }

    public void UpdateBaseAtribute()
    {
        var atk = StatHelper.GetBaseAtribute(beastBase.atk, beastTeamInfo.curStar, beastTeamInfo.curLevel, out _, out _);
        var hp = StatHelper.GetBaseAtribute(beastBase.hp, beastTeamInfo.curStar, beastTeamInfo.curLevel, out _, out _);
        var def = StatHelper.GetBaseAtribute(beastBase.def, beastTeamInfo.curStar, beastTeamInfo.curLevel, out _, out _);

        damageValueTxt.text = MergeBeast.Utils.FormatNumber(atk);
        hpValueTxt.text = MergeBeast.Utils.FormatNumber(hp);
        defValueTxt.text = MergeBeast.Utils.FormatNumber(def);
    }

    private void UpdateStars()
    {
        curMedalTxt.text = MergeBeast.Utils.FormatNumber(beastTeamInfo.curMedal);

        nextMedal = 0;

        for (int k = 0; k < Constant.medalByStars.Count; k++)
        {
            if (beastTeamInfo.curStar < k + 1)
            {
                nextMedal = Constant.medalByStars[k];
                break;
            }
        }
        maxMedalTxt.text = "/" + (nextMedal == 0 ?
            MergeBeast.Utils.FormatNumber(Constant.medalByStars[Constant.medalByStars.Count - 1]) : MergeBeast.Utils.FormatNumber(nextMedal));

        for (int k = 0; k < stars.Count; k++)
        {
            stars[k].gameObject.SetActive(true);
            if (k < beastTeamInfo.curStar)
            {
                stars[k].sprite = starOn;
            }
            else
            {
                stars[k].sprite = starOff;
            }
        }

        if (beastTeamInfo.curMedal < nextMedal || nextMedal == 0)
        {
            // var color = evolveImg.color;
            // color.a = 0.5f;
            // evolveImg.color = color;
            var btn = evolveImg.GetComponent<Button>();
            btn.interactable = false;
            evolveImg.sprite = evolveDisableSp;
        }
        else
        {
            var btn = evolveImg.GetComponent<Button>();
            evolveImg.GetComponent<Button>().interactable = true;
            evolveImg.sprite = evolveEnableSp;
        }

        if (enableType == EnableType.FromListEditTeam)
        {
            // joinTeamImg?.gameObject.SetActive(beastTeamInfo.curStar > 0 && TeamBuildPanel.emptySlotAmout > 0);

            var condition = beastTeamInfo.curStar > 0 && TeamBuildPanel.emptySlotAmout > 0;
            // Debug.Log(_parentTeam.parent.parent.name + " " + TeamBuildPanel.emptySlotAmout + " condition:" + condition);

            joinTeamImg.gameObject.SetActive(condition);
            levelUpImg.gameObject.SetActive(!condition);
            if (!condition) UpdateLevel();
        }
    }

    private void Evolve()
    {
        beastTeamInfo.curMedal -= nextMedal;
        beastTeamInfo.curStar++;
        if (beastTeamInfo.curStar > Constant.medalByStars.Count)
            beastTeamInfo.curStar = Constant.medalByStars.Count;

        if (beastTeamInfo.curStar <= 0) evolveTxt.text = "召唤";
        else evolveTxt.text = "进化";

        Debug.Log("post event BeastEvolve");
        this.PostEvent(EventID.BeastEvolve, (int)beastTeamInfo.beastId);

        UpdateStars();
        UpdateContent2();
        UpdateBaseAtribute();

        // var index = beastTeamInfos.IndexOf(beastTeamInfo);
        // if(index == -1)
        // {
        beastTeamInfo.beastItemViewGo?.GetComponent<BeastTeamItemView>()?.UpdateData(beastTeamInfo);
        beastTeamInfo.beastItemViewGo?.GetComponent<BeastInTeamItemView>()?.UpdateData(beastTeamInfo);
        // }
        // if (enableType == EnableType.FromListEditTeam || enableType == EnableType.FromListMonster)
        // {
        //     parentTeam.GetChild(index).GetComponent<BeastTeamItemView>().UpdateData(beastTeamInfo);
        // }
        // else if (enableType == EnableType.InTeamEditTeam)
        // {
        //     parentTeam.GetChild(index).GetComponent<BeastInTeamItemView>().UpdateData(beastTeamInfo);
        // }

        ActionHelper.BeastEvolve?.Invoke(beastTeamInfo);

        PlayerData.instance.SaveBeastDatas();
    }

    private void UpdateLevel()
    {
        levelTxt.text = beastTeamInfo.curLevel.ToString();
        nextExp = LevelHelper.ExpNeedForNextLevel(beastTeamInfo.curLevel, beastTeamInfo.curLevel + 1);

        curExpTxt.text = MergeBeast.Utils.FormatNumber(PlayerData.instance.CurExp);
        maxExpTxt.text = "/" + MergeBeast.Utils.FormatNumber(nextExp);

        if (PlayerData.instance.CurExp < nextExp || beastTeamInfo.curStar <= 0)
        {
            // var color = levelUpImg.color;
            // color.a = 0.5f;
            // levelUpImg.color = color;

            levelUpImg.GetComponent<Button>().interactable = false;
        }
        else
        {
            // var color = levelUpImg.color;
            // color.a = 1f;
            // levelUpImg.color = color;

            levelUpImg.GetComponent<Button>().interactable = true;
        }
    }

    public System.Numerics.BigInteger LevelUp()
    {
        beastTeamInfo.curLevel++;
        PlayerData.instance.CurExp -= nextExp;

        this.PostEvent(EventID.BeastLvlUp);

        UpdateLevel();
        UpdateContent2();
        UpdateBaseAtribute();

        PlayerData.instance.SaveBeastDatas();

        return beastTeamInfo.curLevel;
    }

    private void UpdateContent2()
    {
        UpdateStats();

        UpdateSkillInfo();
    }

    private void UpdateStats()
    {
        // var langStats = LangManager.Instance.StatsList.Skip(currentStatIndex * 9).Take(9);

        var statInfos = beastTeamInfo.CalculateStats(beastBase);

        for (int k = 0; k < statInfoItemViews.Count; k++)
        {
            var statInfo = statInfos[currentStatIndex * statInfoItemViews.Count + k];

            statInfoItemViews[k].SetUp(LangManager.Instance.Get(statInfo.name), statInfo.totalValueStr, statInfo.moreValueStr, statInfo.morePercentStr);
        }
    }

    private void UpdateSkillInfo()
    {
        for (int i = 0; i < skillHomeInfoItemViews.Count; i++)
        {
            if (beastBase.skillDes.Count > i)
            {
                skillHomeInfoItemViews[i].gameObject.SetActive(true);
                skillHomeInfoItemViews[i].SetUp(this, beastTeamInfo, beastBase.skillDes[i]);
            }
            else
                skillHomeInfoItemViews[i].gameObject.SetActive(false);
        }
    }

    public void ClickStatsBg()
    {
        currentStatIndex++;
        if (currentStatIndex >= 3)
        {
            currentStatIndex = 0;
        }
        UpdateStats();
    }

    public void ClickNextBtn()
    {
        currentBeastIndex++;
        if (currentBeastIndex == beastTeamInfos.Count)
            currentBeastIndex = 0;
        Init(parentTeam, beastTeamInfos[currentBeastIndex], beastTeamInfos, this.enableType);
    }

    public void ClickPrevBtn()
    {
        currentBeastIndex--;
        if (currentBeastIndex == -1)
            currentBeastIndex = beastTeamInfos.Count - 1;
        Init(parentTeam, beastTeamInfos[currentBeastIndex], beastTeamInfos, this.enableType);
    }

    public void Flip()
    {
        currentContent++;
        if (currentContent == contents.Count)
            currentContent = 0;
        SwitchContent();
    }

    public void ClickStashBtn()
    {
        if (beastTeamInfo.curLevel == 0) return;

        var expRetain = LevelHelper.ExpWhenStash(beastTeamInfo.curLevel);
        PlayerData.instance.CurExp += expRetain;

        beastTeamInfo.curLevel = 0;

        UpdateLevel();
        UpdateContent2();
        UpdateBaseAtribute();

        PlayerData.instance.SaveBeastDatas();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void JoinTeam()
    {
        if (parentTeam == null) return;

        if (enableType == EnableType.FromListEditTeam)
        {
            var teamBuildPanel = parentTeam.parent?.parent?.GetComponent<TeamBuildPanel>();
            teamBuildPanel?.AddBeastToTeam(beastTeamInfo);

            // beastTeamInfos.Remove(beastTeamInfo);

            currentBeastIndex--;
            if (currentBeastIndex < 0) currentBeastIndex = 0;

            joinTeamImg.gameObject.SetActive(false);
            levelUpImg.gameObject.SetActive(true);
            UpdateLevel();
        }
    }

    #region tooltip
    public void TouchDownSkill(Vector2 position, string title, bool isPassive, string des, Sprite skillSp, string note1, string note2)
    {
        toolTip.pivot = new Vector3(position.x < Screen.width / 2 ? 0 : 1, 0);
        Debug.Log("TouchDownSkill:" + position + " toolTip.transform.position:" + toolTip.transform.position);

        var wp = new Vector3(position.x, position.y + 50, 1);

        toolTip.transform.position = Camera.main.ScreenToWorldPoint(wp);

        toolTip.GetComponent<SkillToolTipView>().SetUp(title, isPassive, des, skillSp, note1, note2);

        toolTip.gameObject.SetActive(true);
    }

    public void TouchUpSkill(Vector2 position)
    {
        toolTip.pivot = new Vector3(position.x < Screen.width / 2 ? 0 : 1, 0);
        Debug.Log("TouchDownSkill:" + position);

        var wp = new Vector3(position.x, position.y + 50, 1);

        toolTip.transform.position = Camera.main.ScreenToWorldPoint(wp);

        toolTip.gameObject.SetActive(false);
    }
    #endregion

    public void OnclickFillParentBtn()
    {
        gameObject.SetActive(false);
    }

    public void PointerDown(Vector3 position, string title, string des)
    {
        abilityToolTip.pivot = new Vector3(position.x < Screen.width / 2 ? 0 : 1, position.y < Screen.height / 2 ? 0 : 1);
        // Debug.Log("TouchDownSkill:" + position + " toolTip.transform.position:" + toolTip.transform.position);

        var wp = new Vector3(position.x + (position.x < Screen.width / 2 ? 50 : -50), position.y, 1);

        abilityToolTip.transform.position = Camera.main.ScreenToWorldPoint(wp);

        var frame = abilityToolTip.GetChild(0);
        frame.GetChild(0).GetComponent<Text>().text = title;
        frame.GetChild(1).GetComponent<Text>().text = des;

        abilityToolTip.gameObject.SetActive(true);
    }

    public void PointerUp()
    {
        abilityToolTip.gameObject.SetActive(false);
    }
}
