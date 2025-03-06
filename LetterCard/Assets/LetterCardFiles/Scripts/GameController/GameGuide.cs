using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameGuide : MonoBehaviour
{
    public static GameGuide instance;
    public static bool needGuide = false;//是否需要游戏教程

    //对话
    [SerializeField] private GameObject dialoguePanel;//对话Panel
    [SerializeField] private GameObject dialogueBox;//对话框
    [SerializeField] private Text dialogueText;//对话内容
    [SerializeField] private List<string> dialogues=new List<string>();//内容数组
    [SerializeField] private Button dialogueBoxButton;//对话框按钮
    private int dialogueIndex=0;//当前对话数组索引

    // 图文教程
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    [SerializeField] private GameObject imageGuidePanel;
    [SerializeField] private List<GameObject> imageGuides;//图文导航
    [SerializeField] private Button skipGuideButton;//跳过教程按钮
    private int imageGuidesIndex = 0;


    //遮挡按钮的Image
    [SerializeField] private GameObject maskButtonImage;//

    // 右上角目标分数
    [SerializeField] private GameObject targetScoreText;//

    // 上方当前分数
    [SerializeField] private GameObject currentScoreText;//

    // 上方当前回合
    [SerializeField] private GameObject currentRoundText;//

    // 出牌按钮
    [SerializeField] private GameObject playCardButton;//

    // 抽卡按钮
    [SerializeField] private GameObject drawCardButton;//

    //出牌暂存池
    [SerializeField] private GameObject cacheCards;//
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

    }

    public void Init()
    {
        DeckManager.instance.Init();//初始化牌堆
        GetComponent<Canvas>().enabled= true;
        ShowGuideDialogue(dialogues[dialogueIndex]);
        dialogueBoxButton.onClick.AddListener(NextOperation);
    }

    private void ShowGuideDialogue(string dialogue)
    {
        //先打开对话面板
        if(dialoguePanel.activeSelf == false)
        {
            dialoguePanel.SetActive(true);
        }
        dialogueText.text = dialogue;
    }

    private void NextOperation()
    {
        if (dialogueIndex < dialogues.Count-1)
        {
            dialogueIndex++;
            ShowGuideDialogue(dialogues[dialogueIndex]);
        }

        if (dialogueIndex == 2)
        {
            dialoguePanel.GetComponent<Image>().color = new Color32(0, 0, 0, 120);
            targetScoreText.SetActive(true);
        }

        if (dialogueIndex == 3)
        {
            targetScoreText.SetActive(false);
            currentScoreText.SetActive(true);
        }
        if(dialogueIndex == 4)
        {
            currentScoreText.SetActive(false);
            currentRoundText.SetActive(true);
        }
        if(dialogueIndex == 5)
        {
            currentRoundText.SetActive(false);
            dialoguePanel.GetComponent<Image>().color = new Color32(0, 0, 0, 0);

            //dialogueBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
            DeckManager.instance.DrawLetterCard(2);
        }
        if (dialogueIndex == 6)
        {
            dialoguePanel.GetComponent<Image>().raycastTarget = false;
            maskButtonImage.SetActive(true);
            dialogueBoxButton.interactable = false;
            cacheCards.GetComponent<HandCardContainer>().OnTransformChanged += OnCacheChanged;
        }
        if(dialogueIndex == 7)
        {
            dialoguePanel.GetComponent<Image>().color = new Color32(0, 0, 0, 150);
            playCardButton.SetActive(true);
        }
        if (dialogueIndex == 8)
        {
            playCardButton.SetActive(false);
            currentScoreText.SetActive(true);
            currentScoreText.GetComponent<Text>().text = "2";
        }
        if (dialogueIndex == 9) 
        {
            currentScoreText.SetActive(false);
            drawCardButton.SetActive(true);

            GameEntrance.instance.CoinCount = 2;
            GameManager.Instance.DrawNeedCoin = 1;
            cacheCards.GetComponent<HandCardContainer>().OnTransformChanged=null;
            Debug.Log("游戏教程");
        }
    }

    private void OnCacheChanged()
    {
        if (cacheCards.transform.childCount == 2)
        {
            dialoguePanel.GetComponent<Image>().raycastTarget = true;
            maskButtonImage.SetActive(false);
            NextOperation();
        }
    }

    public void OnPlayeButtonClick()
    {
        dialogueBoxButton.interactable = true;
        NextOperation();
    }

    public void OnDrawCardButtonClick()
    {
        //开始图文教程
        // 创建一个动画序列
        Sequence sequence = DOTween.Sequence();
        
        sequence.AppendCallback(() =>
        {
            dialoguePanel.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
        });
        // 添加停顿一秒
        sequence.AppendInterval(2.0f);  // 停顿

        sequence.AppendCallback(() =>
        {
            imageGuidePanel.SetActive(true);
            dialoguePanel.SetActive(false);
        });
    }

    public void NextGuide()
    {
        imageGuidesIndex++;
        if(imageGuidesIndex>= imageGuides.Count-1)
        {
            nextButton.interactable= false;
            skipGuideButton.gameObject.SetActive(true);
        }
        previousButton.interactable= true;

        imageGuides[imageGuidesIndex - 1].GetComponent<RectTransform>().DOAnchorPosX(-1000, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex - 1].GetComponent<RectTransform>().DOScale(0, 0.5f).SetEase(Ease.OutQuad);

        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().anchoredPosition=new Vector2(1000,0);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(0, 0.01f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad);
    }

    public void PreciousGuide()
    {
        imageGuidesIndex--;
        if (imageGuidesIndex <= 0)
        {
            previousButton.interactable = false;
            
        }
        nextButton.interactable = true;
        skipGuideButton.gameObject.SetActive(false);

        imageGuides[imageGuidesIndex + 1].GetComponent<RectTransform>().DOAnchorPosX(1000, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex + 1].GetComponent<RectTransform>().DOScale(0, 0.5f).SetEase(Ease.OutQuad);

        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().anchoredPosition = new Vector2(-1000, 0);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(0, 0.01f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutQuad);
        imageGuides[imageGuidesIndex].GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad);
    }

    public void SkipGuide()
    {
        GetComponent<Canvas>().enabled = false;
        GameManager.Instance.StartChallenge();
        gameObject.SetActive(false);
    }
}
