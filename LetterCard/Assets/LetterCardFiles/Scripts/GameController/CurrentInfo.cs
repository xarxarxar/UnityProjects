using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CurrentInfo : MonoBehaviour
{
    public static CurrentInfo Instance;
    private void Awake()
    {
        Instance = this;
    }

    public Text currentScoreText;
    public Text nextScoreText;
    public Text letterCardMaxText;
    public Text specialCardMaxText;
    public Text continousCountText;
    public Text continousProbText;
    public Text extraScoreOnlyOneText;
    public Text canPlayZeroCardText;
    public Text addScoreWhenDeleteText;
    public Text extraScoreRounOverText;

    /// <summary>
    /// 关闭信息面板
    /// </summary>
    public void CloseCurrentInfoPanel()
    {
        transform.DOScale(0,0.2f).SetEase(Ease.OutQuart);
        GetComponent<Canvas>().enabled = false;
    }

    [Header("暂停相关")]
    [SerializeField] private Button settingButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    /// <summary>
    /// 设置按钮
    /// </summary>
    public void SettingButton()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        SettingManager.Instance.OpenCurrentPanel();
    }
    
    /// <summary>
    /// 继续按钮
    /// </summary>
    public void ContinueButton()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 重新开始按钮
    /// </summary>
    public void RestartButton()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GetComponent<Canvas>().enabled = false;
        GameManager.Instance.StartChallenge();
    }

    /// <summary>
    /// 主页按钮
    /// </summary>
    public void HomeButton()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        DeckManager.instance.ClearHandCards();
        GameEntrance.instance.GetComponent<Canvas>().enabled = true;
        GameManager.Instance.GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = false;
    }

}
