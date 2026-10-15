using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// SingleLevelUI：关卡选择界面中单个关卡按钮的 UI 状态管理。
/// 由调用方（关卡选择面板）在显示前设置 chapter / levelDifficulty / level / startCount 后调用 Init()。
/// </summary>
public class SingleLevelUI : MonoBehaviour
{
    /// <summary>当前关卡是第几章。</summary>
    public int chapter;

    /// <summary>当前关卡是什么难度（Normal / Hard / ExtremelyHard）。</summary>
    public LevelDifficulty levelDifficulty;

    /// <summary>当前关卡是第几关。</summary>
    public int level;

    /// <summary>
    /// 当前关卡是几星通关，0星通关也算通关。
    /// 该值为 -1 表示此关已解锁但未通关，该值为 -2 表示此关未解锁。
    /// </summary>
    public int startCount;

    /// <summary>锁，未解锁时显示。</summary>
    public GameObject lockObject;

    /// <summary>星星物体，数组大小为 3，根据该关卡的星级确定是否显示。</summary>
    public GameObject[] starObject;

    /// <summary>关卡按钮，点击之后进入该关卡。</summary>
    public Button levelButton;

    /// <summary>是否已通关（startCount >= 0，0 星通关也算通关）。</summary>
    public bool HASPASSED => startCount >= 0;

    /// <summary>是否已解锁（startCount > -2）。</summary>
    public bool HASUNLOCKED => startCount > -2;

    /// <summary>
    /// 全参初始化：设置章节/难度/关卡/星级状态并刷新 UI。
    /// starCount 取值：-2 未解锁、-1 已解锁未通关、0~3 已通关星级。
    /// 由关卡选择面板在显示/刷新关卡按钮时调用。
    /// </summary>
    public void Init(int chapter, LevelDifficulty difficulty, int level, int starCount)
    {
        this.chapter = chapter;
        this.levelDifficulty = difficulty;
        this.level = level;
        this.startCount = starCount;
        RefreshUI();
        Debug.Log($"当前关卡为第{chapter}章{difficulty}模式第{level}关，是否已解锁：{HASUNLOCKED}");
    }

    /// <summary>按当前字段刷新锁、星星与按钮可点击状态（字段赋值后调用）。</summary>
    public void RefreshUI()
    {
        bool unlocked = HASUNLOCKED;
        bool passed = HASPASSED;

        // 锁：未解锁显示
        if (lockObject != null)
        {
            lockObject.SetActive(!unlocked);
        }

        // 星星：已通关显示前 startCount 颗（0 星通关不显示）；未解锁/未通关全部隐藏
        int starsToShow = passed ? Mathf.Clamp(startCount, 0, 3) : 0;
        if (starObject != null)
        {
            for (int i = 0; i < starObject.Length; i++)
            {
                if (starObject[i] != null)
                {
                    starObject[i].SetActive(i < starsToShow);
                }
            }
        }

        // 按钮：未解锁不可点击
        if (levelButton != null)
        {
            levelButton.interactable = unlocked;
        }
    }

    /// <summary>设置本关卡星级状态（-2 未解锁 / -1 已解锁未通关 / 0~3 星已通关）并刷新 UI。</summary>
    public void SetState(int starCount)
    {
        startCount = starCount;
        RefreshUI();
    }

    /// <summary>设置为未解锁并刷新 UI。</summary>
    public void SetLocked()
    {
        startCount = -2;
        RefreshUI();
    }

    /// <summary>设置为已解锁未通关并刷新 UI。</summary>
    public void SetUnlocked()
    {
        startCount = -1;
        RefreshUI();
    }

    /// <summary>设置为已通关（星星数）并刷新 UI。</summary>
    public void SetPassed(int stars)
    {
        startCount = Mathf.Max(0, stars);
        RefreshUI();
    }
}
