using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏入口
/// </summary>
public class GameEntrance : MonoBehaviour
{
    private static uint coinCount = 100;//金币的数量

    public static uint CoinCount { get => coinCount; set => coinCount = value; }

    //UI

    // Start is called before the first frame update
    void Start()
    {
        ButtonManager.instance.startGameButton.onClick.AddListener(SartGame);
    }

    private void OnEnable()
    {

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
        //GameManager.Instance.StartChallenge();//开始挑战
    }

    /// <summary>
    /// 设置金币的Text
    /// </summary>
    private void SetCoinText()
    {
        
    }

}
