using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteLevelManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private ScrollRect scrollRect;

    [Header("Generation Settings")]
    [SerializeField] private int initialPoolSize = 20; // 初始生成的关卡按钮数量
    [SerializeField] private int loadThreshold = 5;    // 提前加载阈值

    private int currentMaxLevel = 1;  // 当前最大关卡
    private int unlockedLevel = 1;    // 当前解锁的关卡
    private List<LevelButton> buttonList = new List<LevelButton>();

    // 玩家数据存储结构
    private class PlayerProgress
    {
        public int unlockedLevel = 1;
        public Dictionary<int, int> levelScores = new Dictionary<int, int>();
    }
    private PlayerProgress progress;

    void Start()
    {
        LoadProgress();
        GenerateInitialLevels();
        StartCoroutine(MonitorScrollPosition());
        UpdateButtonStates();
    }

    // 生成初始关卡
    private void GenerateInitialLevels()
    {
        for (int i = 1; i <= initialPoolSize; i++)
        {
            CreateLevelButton(i);
        }
        currentMaxLevel = initialPoolSize;
    }

    // 创建关卡按钮
    private void CreateLevelButton(int levelNumber)
    {
        GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
        LevelButton button = buttonObj.GetComponent<LevelButton>();
        int bestScore = progress.levelScores.ContainsKey(levelNumber) ? progress.levelScores[levelNumber] : 0;
        button.Initialize(levelNumber, levelNumber <= unlockedLevel, bestScore, () => OnLevelButtonClick(levelNumber));
        buttonList.Add(button);
    }

    // 处理关卡按钮点击事件
    private void OnLevelButtonClick(int levelNumber)
    {
        if (levelNumber <= unlockedLevel)
        {
            // 在这里实现进入关卡的逻辑
            GameObject.Find("MainCanvas").GetComponent<Canvas>().enabled = false;
            LevelController.instance.StartLevel(levelNumber);
        }
        else
        {
            Debug.Log("This level is locked.");
        }
    }

    // 每次滚动时检查是否需要加载更多关卡
    private IEnumerator MonitorScrollPosition()
    {
        while (true)
        {
            float normalizedPosition = 1 - scrollRect.verticalNormalizedPosition;
            int visibleMaxLevel = Mathf.FloorToInt(normalizedPosition * currentMaxLevel);

            if (currentMaxLevel - visibleMaxLevel < loadThreshold)
            {
                LoadMoreLevels(10); // 每次加载10个新关卡
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    // 加载更多关卡
    private void LoadMoreLevels(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            CreateLevelButton(currentMaxLevel + i);
        }
        currentMaxLevel += count;
    }

    // 解锁下一个关卡
    public void UnlockNextLevel()
    {
        unlockedLevel++;
        SaveProgress();
        UpdateButtonStates();
    }

    // 更新按钮的锁定状态
    private void UpdateButtonStates()
    {
        foreach (var button in buttonList)
        {
            button.UpdateLockState(button.LevelNumber > unlockedLevel);
        }
    }

    #region Progress Management
    private void LoadProgress()
    {
        string json = PlayerPrefs.GetString("PlayerProgress", "");
        if (!string.IsNullOrEmpty(json))
        {
            progress = JsonUtility.FromJson<PlayerProgress>(json);
        }
        else
        {
            progress = new PlayerProgress();
        }
        unlockedLevel = progress.unlockedLevel;
    }

    private void SaveProgress()
    {
        progress.unlockedLevel = unlockedLevel;
        string json = JsonUtility.ToJson(progress);
        PlayerPrefs.SetString("PlayerProgress", json);
        PlayerPrefs.Save();
    }
    #endregion
}
