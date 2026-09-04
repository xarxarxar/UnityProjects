using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class RewardData
{
    public RewardType rewardType;
    public int amount;
}

[System.Serializable]
public class MileStone
{
    public List<RewardData> rewards;
    public int starAmount;
}

[System.Serializable]
public class ChapterData
{
    public int chapter;
    public string title;
    public List<MileStone> milestones;
}

[CreateAssetMenu(fileName = "ChapterDatas", menuName = "CrushDatas/ChapterDatas")]
public class ChapterDatas : ScriptableObject
{
    public List<ChapterData> chapterDatas;

    public ChapterData GetChapterData(int chapter)
    {
        var chapterData = chapterDatas.FirstOrDefault(md => md.chapter == chapter);
        if (chapterData == null) chapterData = chapterDatas.First();
        return chapterData;
    }
}
