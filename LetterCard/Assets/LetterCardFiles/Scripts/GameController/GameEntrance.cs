using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeChatWASM;

/// <summary>
/// 游戏入口
/// </summary>
public class GameEntrance : MonoBehaviour
{
    public static GameEntrance instance;
    private int coinCount;//金币的数量
    public int CoinCount 
    { 
        get => coinCount;
        set 
        { 
            coinCount = value;
            if (value < 0)
            {
                coinText.text = "--";
            }
            else
            {
                coinText.text = coinCount.ToString();
            }
            
            GameManager.Instance.coinText.text = coinCount.ToString();
            if (value < GameManager.Instance.DrawNeedCoin)
            {
                GameManager.Instance.needCoinText.color = new Color32(255, 34, 12, 255);//红色
            }
            else
            {
                GameManager.Instance.needCoinText.color = new Color32(255, 255, 255, 255);//红色
            }
        }
    }

    [SerializeField] private Text coinText;

    public Image startGameButton;//一开始的开始游戏按钮

    [SerializeField] private GameObject enterGamePanel;//进入游戏界面

    //UI

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初始化微信 SDK
        WX.InitSDK(
            (code) =>
            {
                ShowTipManager.instance.ShowLoading(true);
                WX.cloud.Init(new ICloudConfig()
                {
                    env = "cloud1-1g93cld7637aacb4", // 云环境 ID
                    traceUser = false
                });
                ShowTipManager.instance.ShowLoading(false);
                WechatManager.instance.CreateUserInfoButtonBefore();//先创建获取用户信息按钮
                WechatManager.instance.CreateUserInfoButton();
            }
        );
        ButtonManager.instance.startGameButton.onClick.AddListener(SartGame);
    }


    /// <summary>
    /// 开始游戏
    /// </summary>
    public void SartGame()
    {
        AudioManager.instance.PlaySoundEffect("ClickButton");
        GetComponent<Canvas>().enabled = false;//关闭主界面

        GameManager.Instance.GetComponent<Canvas>().enabled = true;

        if (DataManager.instance.globalPlayerInfo.needGuide)
        {
            GameGuide.instance.Init();
        }
        else
        {
            GameManager.Instance.StartChallenge();//开始挑战
        }
    }

    /// <summary>
    /// 添加金币按钮
    /// </summary>
    public void AddMoneyButton()
    {
        GetRewards.Instance.GetComponent<Canvas>().enabled = true;
    }

    /// <summary>
    /// 设置金币的Text
    /// </summary>
    private void SetCoinText()
    {
        
    }

    public void CloseEnterGamePanel()
    {
        enterGamePanel.SetActive(false);
    }

}
