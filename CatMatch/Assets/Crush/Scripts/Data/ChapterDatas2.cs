using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChapterDatas", menuName = "CrushDatas/ChapterDatas")]
public class ChapterDatas2 : MonoBehaviour
{
    public List<ChapterData> chapterDatas;

    // public ChapterData GetChapterData(int chapter)
    // {
    //     var chapterData = chapterDatas.FirstOrDefault(md => md.chapter == chapter);
    //     if (chapterData == null) chapterData = chapterDatas.FirstOrDefault();
    //     return chapterData;
    // }
}
