using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Observer;
using UnityEngine;
using UnityEngine.UI;

public enum ClaimType
{
    Cant,
    Can,
    Claimed
}

public class ChapterRewardPanel : MonoBehaviour
{
    public Text titleTxt;
    public Text startTxt;
    public Transform rewardParent;
    public Button claimBtn;
    public GameObject chapterRewardPref;
    public Button prevBtn;
    public Button nextBtn;
    public Image doneClaimImg;
    public PanelReceive2 panelReceive2;

    private int currentChapter, maxChapter, currentMileStone;

    void Awake()
    {
        this.RegisterListener(EventID.EndOnClickReceiveShopTut, (sender, param) => EndOnClickReceiveShopTut());
    }

    private void EndOnClickReceiveShopTut()
    {
        Init();
    }

    void OnEnable()
    {
        int maxStage = PlayerData.instance.MaxStage;
        int currentRegion = (maxStage - 1) / PlayerData.instance.MaxStagePerRegion;
        currentChapter = currentRegion + 1;
        maxChapter = currentChapter;

        Init();
    }

    public void Init()
    {
        var myTotalStar = RewardHelper.GetTotalStarOfChapter(currentChapter - 1);

        ClaimType claimType = ClaimType.Cant;
        currentMileStone = 0;
        var chapterData = GameData.Instance.chapterDatas.GetChapterData(currentChapter);
        var receivedMileStones = PlayerPrefs.GetString(CrushStringHelper.LastReceiveMileStoneReward, "").Split('|').ToList();
        for (int k = 0; k < receivedMileStones.Count; k++)
        {
            int.TryParse(receivedMileStones[k], out var receiveMileStone);
            if (receiveMileStone > 0)
            {
                var revChapter = receiveMileStone / 100;
                if (revChapter == currentChapter)
                {
                    currentMileStone = receiveMileStone % 100;
                }
            }
        }
        var nextMileStone = currentMileStone + 1;
        Debug.Log("nextMileStone:" + nextMileStone + " chapterData.milestones.Count:" + chapterData.milestones.Count);

        if (nextMileStone > chapterData.milestones.Count)
        {
            claimType = ClaimType.Claimed;
            nextMileStone--;
        }
        else
        {
            if (myTotalStar >= chapterData.milestones[nextMileStone - 1].starAmount)
            {
                claimType = ClaimType.Can;
            }
        }

        titleTxt.text = chapterData.title;
        startTxt.text = myTotalStar + "/" + chapterData.milestones[nextMileStone - 1].starAmount;

        while (rewardParent.childCount > 0)
        {
            var go = rewardParent.GetChild(0).gameObject;
            ObjectPool.Instance.ReleaseObject(go);
        }
        Debug.Log("nextMileStone:" + nextMileStone);
        var datas = chapterData.milestones[nextMileStone - 1].rewards;
        for (int k = 0; k < datas.Count; k++)
        {
            var go = ObjectPool.Instance.GetGameObject(chapterRewardPref, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(rewardParent);
            go.transform.localScale = new UnityEngine.Vector3(1, 1, 1);
            // Debug.Log("Rewards amount:" + datas[k].amount);
            go.GetComponent<RewardChapterItemView>().SetUp(datas[k]);
        }

        if (claimType == ClaimType.Claimed)
        {
            doneClaimImg.gameObject.SetActive(true);
            claimBtn.gameObject.SetActive(false);
        }
        else
        {
            doneClaimImg.gameObject.SetActive(false);
            claimBtn.gameObject.SetActive(true);
            claimBtn.interactable = claimType == ClaimType.Can;
            claimBtn.GetComponentInChildren<Text>().text = claimType == ClaimType.Claimed ? "已领取" : "领取";
        }

        var nextImg = nextBtn.transform.GetChild(0).GetComponent<Image>();
        var prevImg = prevBtn.transform.GetChild(0).GetComponent<Image>();

        Debug.Log("currentChapter:" + currentChapter + " maxChapter:" + maxChapter);

        if (currentChapter < maxChapter)
        {
            nextBtn.interactable = true;
            var color = nextImg.color;
            color.a = 1f;
            nextImg.color = color;
        }
        else
        {
            nextBtn.interactable = false;
            var color = nextImg.color;
            color.a = 0.5f;
            nextImg.color = color;
        }
        if (currentChapter > 1)
        {
            prevBtn.interactable = true;
            var color = prevImg.color;
            color.a = 1f;
            prevImg.color = color;
        }
        else
        {
            prevBtn.interactable = false;
            var color = prevImg.color;
            color.a = 0.5f;
            prevImg.color = color;
        }
    }

    public void OnClickClaimBtn()
    {
        var receivedMileStones = PlayerPrefs.GetString(CrushStringHelper.LastReceiveMileStoneReward, "");
        Debug.Log("receivedMileStones:" + receivedMileStones);
        receivedMileStones += (currentChapter * 100 + currentMileStone + 1) + "|";
        Debug.Log("receivedMileStones 2:" + receivedMileStones);
        PlayerPrefs.SetString(CrushStringHelper.LastReceiveMileStoneReward, receivedMileStones);

        var chapterData = GameData.Instance.chapterDatas.GetChapterData(currentChapter);
        List<RewardModel> rewardModels = new List<RewardModel>();

        for (int k = 0; k < chapterData.milestones[currentMileStone].rewards.Count; k++)
        {
            var data = chapterData.milestones[currentMileStone].rewards[k];
            RewardHelper.ReceiveRewards(data, true);
            rewardModels.Add(new RewardModel() { rewardType = data.rewardType, amount = data.amount });
        }
        panelReceive2.gameObject.SetActive(true);
        panelReceive2.Init(rewardModels);
        
        // this.PostEvent(EventID.BackToMap, null);

        MergeBeast.MapController._Instance?.ShowRegions();

        // Init();
    }

    public void NextChapter()
    {
        currentChapter++;
        Init();
    }

    public void PrevChapter()
    {
        currentChapter--;
        Init();
    }
}
