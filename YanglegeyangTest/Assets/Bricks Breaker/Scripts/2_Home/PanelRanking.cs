using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.Networking;


public class RankingData
{
    public int rank;
    public string countryName;
    public string userName;
    public int score;
    public int turn;
}

public class PanelRanking : PanelBase
{

    public RankingList myRankingList;
    public Transform content;
    public GameObject pRankingList;
    public List<RankingList> rankingLists = new List<RankingList>();
    public List<RankingData> rankingDatas = new List<RankingData>();

    public CanvasGroup canvasGroupLoading;
    public Transform loading;
    public Sprite[] flagSprites;

    public bool isRankingDataLoad = false;


    //Streaming Asset Folder Path
#if UNITY_EDITOR
    string streamingPath = Application.streamingAssetsPath;
#elif UNITY_IOS
    string streamingPath = Application.dataPath + "/Raw";
#elif UNITY_ANDROID
    string streamingPath = Application.streamingAssetsPath;
#endif

    /// <summary>
    /// Initialize all UI.
    /// </summary>
    public void UIReset()
    {
        canvasGroup.transform.DOScale(0.95f, 0f);
        canvasGroup.DOFade(0f, 0f);
        content.gameObject.SetActive(false);

        for (int i = 0; i < 100; i++)
        {
            GameObject list = Instantiate(pRankingList);
            list.transform.SetParent(content, false);
            list.transform.SetAsLastSibling();
            list.SetActive(false);
            rankingLists.Add(list.GetComponent<RankingList>());
        }
    }

    public void GetData()
    {
        StartCoroutine(GeteDataCo());
    }

    IEnumerator GeteDataCo()
    {
        yield return StartCoroutine(GetRankingDataCo());
        isRankingDataLoad = true;
    }

    //Retrieve the RankingRankingData.json ranking data in the StreamingAssets folder and put it in the rankingDatas list.
    IEnumerator GetRankingDataCo()
    {
        string path = streamingPath + "/SampleRankingData.json";
        string jsonString;
        if (path.Contains("://") || path.Contains(":///"))
        {
            UnityWebRequest www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
            jsonString = www.downloadHandler.text;
        }
        else
        {
            jsonString = File.ReadAllText(path);
        }

        rankingDatas = JsonConvert.DeserializeObject<List<RankingData>>(jsonString);
    }



    public void ShowRanking()
    {
        Loading(true);

        //Internet check for server communication
        // if (!PlayManager.Instance.IsInternet()) return;

        content.gameObject.SetActive(false);
        StartCoroutine(LoadRankingScoreCo());
    }


    //todo 데이터가져오기
    IEnumerator LoadRankingScoreCo()
    {
        GetData();
        while (!isRankingDataLoad) yield return null;

        //Setting my ranking information
        myRankingList.SetList(
            999,
            GetLangFlag("kr"),
            GameData.NickName,
            GameData.BestScore,
            GameData.Turn
        );


        //List of all ranking information
        for (int i = 0; i < rankingDatas.Count; i++)
        {
            rankingLists[i].SetList(
                rankingDatas[i].rank,
                GetLangFlag(rankingDatas[i].countryName),
                rankingDatas[i].userName,
                rankingDatas[i].score,
                rankingDatas[i].turn
            );

            //Change the color of my list in the overall score
            //if(data.rank == i+1) {
            //    rankingLists[i].textName.color = PlayManager.Instance.HexToColor("FFF028");
            //}

        }

        yield return null;

        //Loading complete
        content.gameObject.SetActive(true);
        Loading(false);
    }


   /// <summary>
   /// LoadingAnimation
   /// </summary>
    void Loading(bool value)
    {
        if (value)
        {
            canvasGroupLoading.gameObject.SetActive(true);
            loading.transform.DORotate(new Vector3(0f, 0f, -360f), 1f).SetRelative(true).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart).SetUpdate(true);
            canvasGroupLoading.DOKill();
            canvasGroupLoading.DOFade(1f, 0.25f).SetEase(Ease.OutSine).SetUpdate(true).SetUpdate(true);
        }
        else
        {
            canvasGroupLoading.DOKill();
            canvasGroupLoading.DOFade(0f, 0.15f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                loading.transform.DOKill();
                canvasGroupLoading.gameObject.SetActive(false);
            }).SetUpdate(true);
        }
    }



    /// <summary>
    /// Flag already imported with country code
    /// </summary>
    public Sprite GetLangFlag(string code)
    {
        string res = code.ToLower();
        res.ToLower();

        foreach (Sprite sprite in flagSprites)
        {

            if (sprite.name == res)
            {
                return sprite;
            }
        }

        return null;
    }
}
