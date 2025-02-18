using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteLevelManager : MonoBehaviour
{
    public static InfiniteLevelManager instance;
    [Header("UI Components")]
    [SerializeField] private Transform levelButtonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private ScrollRect scrollRect;

    private int currentMaxLevel = 1;  // 当前最大关卡
    private int unlockedLevel = 1;    // 当前解锁的关卡
    private List<LevelButton> buttonList = new List<LevelButton>();

    //页面
    private int currentPage = 1;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    public int CurrentPage 
    { 
        get => currentPage;
        set 
        {
            if (currentPage != value)
            {
                currentPage = value;
            }
            previousButton.gameObject.SetActive(currentPage != 1);
        } 
    }

    PlayerInfo playInfo;

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        GenerateInitialLevels();
        UpdateButtonStates();

        
    }

    // 生成初始关卡
    private void GenerateInitialLevels()
    {
        //初始化按钮状态
        previousButton.onClick.AddListener(PreviousPage);
        nextButton.onClick.AddListener(NextPage);
        CurrentPage = 1;

        for (int i = 1; i <= 15; i++)
        {
            CreateLevelButton(i);
        }
        currentMaxLevel = 1;
    }

    // 创建关卡按钮
    private void CreateLevelButton(int levelNumber)
    {
        //Debug.Log($"levelNumber is {levelNumber}");
        LevelButton foundButton = buttonList.Find(button => button.LevelNumber == levelNumber);
        if (foundButton!=null)//已存在
        {
            return;
        }
        if (buttonList.Count < 15)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            LevelButton button = buttonObj.GetComponent<LevelButton>();
            button.Initialize(levelNumber, levelNumber > unlockedLevel, () => OnLevelButtonClick(levelNumber));
            buttonList.Add(button);
        }
        else
        {
            LevelButton button = buttonList[(levelNumber-1) % 15 ];
            button.Initialize(levelNumber, levelNumber >unlockedLevel, () => OnLevelButtonClick(levelNumber));
        }
        
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


    /// <summary>
    /// 下一页
    /// </summary>
    public void NextPage()
    {
        CurrentPage++;
        for (int i = 15*(CurrentPage-1)+1; i <= 15 * (CurrentPage - 1) + 15; i++)
        {
            CreateLevelButton(i);
        }
    }

    public void PreviousPage()
    {
        if (CurrentPage == 1) return;
        CurrentPage --;
        for (int i = 15 * (CurrentPage - 1) + 1; i <= 15 * (CurrentPage - 1) + 15; i++)
        {
            CreateLevelButton(i);
        }
    }

    // 解锁下一个关卡
    public void UnlockNextLevel()
    {
        unlockedLevel++;
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

    #endregion
}
