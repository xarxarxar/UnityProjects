using UnityEngine;
using UnityEngine.UI;

public class BankPanel : BasePanel
{
    [SerializeField] private Text _bankCoinText;

    public override void OnEnable()
    {
        base.OnEnable();
        CloseButton.onClick.AddListener(CloseButtonClick);
        //BattleManager.Instance.PauseGame();//暂停游戏
        _bankCoinText.text = $"当前存款:{BankManager.Instance.Count}";
    }

    private void OnDisable()
    {
        //BattleManager.Instance.ResumeGame();//恢复游戏
        CloseButton.onClick.RemoveAllListeners();
    }

    //关闭当前面板
    private void CloseButtonClick()
    {
        BattleUIManager.Instance.HidewBankPanel();
    }
}
