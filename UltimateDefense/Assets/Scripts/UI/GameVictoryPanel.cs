using UnityEngine;
using UnityEngine.UI;

public class GameVictoryPanel : MonoBehaviour
{
    [SerializeField] private Button _continueButton;//继续按钮

    private void Start()
    {
        _continueButton.onClick.AddListener(ContinueButton);
    }

    //继续按钮的点击事件
    private void ContinueButton()
    {
        gameObject.SetActive(false);
        BattleUIManager.Instance.HideBattleScene();
        GameUIManager.Instance.ShowMainMenu();//返回到主菜单
    }
}
