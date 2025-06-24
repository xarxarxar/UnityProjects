using UnityEngine;
using UnityEngine.UI;

public class EnterPanel : MonoBehaviour
{
    public Button EnterButton;//进入游戏按钮

    private void Start()
    {
        EnterButton.onClick.AddListener(EnterGameButton);
    }

    private void OnDisable()
    {
        EnterButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// 进入游戏按钮点击方法
    /// </summary>
    private void EnterGameButton()
    {
        Debug.Log("点击进入游戏按钮");
        GameManager.Instance.LoadNewSceneAsync("GameScene");
    }

    
}
