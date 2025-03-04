using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏入口
/// </summary>
public class GameEntrance : MonoBehaviour
{
    public static GameEntrance instance;
    private uint coinCount;//金币的数量
    public uint CoinCount 
    { 
        get => coinCount;
        set 
        { 
            coinCount = value;
            coinText.text = coinCount.ToString();
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

    //UI

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CoinCount = 10;
        ButtonManager.instance.startGameButton.onClick.AddListener(SartGame);
    }


    /// <summary>
    /// 开始游戏
    /// </summary>
    public void SartGame()
    {
        GetComponent<Canvas>().enabled = false;//关闭主界面

        GameManager.Instance.GetComponent<Canvas>().enabled = true;
        if (GameGuide.needGuide)
        {
            GameGuide.instance.Init();
        }
        else
        {
            GameManager.Instance.StartChallenge();//开始挑战
        }
        
    }

    /// <summary>
    /// 设置金币的Text
    /// </summary>
    private void SetCoinText()
    {
        
    }

}
