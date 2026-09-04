using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    private static GameData instance;
    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("GameData");
                instance = gameObject.AddComponent<GameData>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
        private set { }
    }

    public StageDatas stageDatas;
    public RewardSprites rewardSprites;
    public ChapterDatas chapterDatas;
    public MonsterDesSprites monsterDesSprites;
    public StatusSprites statusSprites;
    public CritSprites critSprites;

    private void Awake()
    {
        if (instance != null && instance.GetInstanceID() != this.GetInstanceID())
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as GameData;
            DontDestroyOnLoad(gameObject);
        }

        // runIE = run();
        // StartCoroutine(runIE);
        // StartCoroutine(run1());
    }

    void OnEnable()
    {
        var lastOnLineTimeStr = PlayerPrefs.GetString(CrushStringHelper.LastOnLineDateTime, "");
        if (string.IsNullOrEmpty(lastOnLineTimeStr))
        {
            lastOnLineTimeStr = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            Debug.Log("lastOnLineTimeStr:" + lastOnLineTimeStr);
            PlayerPrefs.SetString(CrushStringHelper.LastOnLineDateTime, lastOnLineTimeStr);
        }
    }

    IEnumerator runIE;


    IEnumerator run1()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (runIE == null)
            {
                Debug.Log("wtf");
            }

        }
    }
    IEnumerator run()
    {
        yield return new WaitForSeconds(1f);
        // Debug.Log("End run");
    }
}
