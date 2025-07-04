using UnityEngine;
using UnityEngine.UI;

public class BankPanel : MonoBehaviour
{
    [SerializeField] private Text _bankCoinText;
    [SerializeField] private Button _closeButton;//关闭当前面板的按钮

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(CloseButton);
       BattleManager.Instance.PauseGame();//暂停游戏
        _bankCoinText.text = $"当前存款:{BankManager.Instance.Count}";
    }

    private void OnDisable()
    {
        BattleManager.Instance.ResumeGame();//恢复游戏
        _closeButton.onClick.RemoveAllListeners();
    }

    //关闭当前面板
    private void CloseButton()
    {
        BattleUIManager.Instance.HidewBankPanel();
    }
}
