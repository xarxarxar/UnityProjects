using UnityEngine;
using UnityEngine.UI;

public class GameFailCanvas : MonoBehaviour
{
    //主页按钮
    [SerializeField] private Button menuButton;
    //重玩按钮
    [SerializeField] private Button replayButton;
    //继续挑战按钮
    [SerializeField] private Button continueButton;

    /// <summary>
    /// 回到主页
    /// </summary>
    public void BackToMenu()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        DeckManager.instance.ClearHandCards();
        GameEntrance.instance.GetComponent<Canvas>().enabled = true;
        GameManager.Instance.GetComponent<Canvas>().enabled = false;
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 重新挑战
    /// </summary>
    public void RestartChallenge()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GameManager.Instance.StartChallenge();
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// 继续挑战
    /// </summary>
    public void ContinueChallenge()
    {
        //GameManager.Instance.ContinueChallenge();
        //GameManager.Instance.DoubleScoreRoundCount = 5;
        AudioManager.instance.PlaySoundEffect("ClickButton");
        WechatManager.ShareApp(() =>
        {
            GameManager.Instance.ContinueChallenge();
            GameManager.Instance.DoubleScoreRoundCount = 5;
        });

    }

}
