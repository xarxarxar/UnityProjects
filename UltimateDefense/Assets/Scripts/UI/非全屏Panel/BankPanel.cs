using UnityEngine;
using UnityEngine.UI;

public class BankPanel : BasePanel
{
    [SerializeField] private Text _saveCoinText;//已存的金币文本
    [SerializeField] private Text _nextRoundBackText;//下回合返还的金币文本
    [SerializeField] private Text _saveInterestText;//存款利息
    [SerializeField] private Text _maxAdvanceCoinText;//已借的金币文本
    [SerializeField] private Text _needReturnCoinText;//待还的金币文本
    [SerializeField] private Text _advanceInterestText;//借款利息

    [SerializeField] private Button _saveMoneyButton;//存款按钮-暂定只能每500一存
    [SerializeField] private Button _advanceButton;//借款按钮-暂定只能每500一借

    private void Start()
    {
        _saveCoinText.text = BankManager.Instance.CurrentSave.Value.ToString();
        _saveInterestText.text = $"{(BankManager.Instance.SaveInterest.Value - 1) * 100}%";
        _nextRoundBackText.text = $"{BankManager.Instance.NextRoundGetMoney.Value}";
        _maxAdvanceCoinText.text = $"{BankManager.Instance.MaxAdvance.Value}";
        _needReturnCoinText.text = $"{BankManager.Instance.CurrentNeedReturn.Value}";
        _advanceInterestText.text = $"{(BankManager.Instance.AdvanceInterest.Value - 1) * 100}%";
        //存
        BankManager.Instance.CurrentSave.OnValueChanged+= (value) =>
        {
            _saveCoinText.text = BankManager.Instance.CurrentSave.Value.ToString();
        };
        BankManager.Instance.SaveInterest.OnValueChanged += (value) =>
        {
            _saveInterestText.text = $"{(BankManager.Instance.SaveInterest.Value - 1) * 100}%";
        };
        BankManager.Instance.NextRoundGetMoney.OnValueChanged += (value) =>
        {
            _nextRoundBackText.text = $"{BankManager.Instance.NextRoundGetMoney.Value}";
        };

        //借
        BankManager.Instance.CurrentNeedReturn.OnValueChanged += (value) =>
        {
            _maxAdvanceCoinText.text= $"{BankManager.Instance.MaxAdvance.Value}";
            _needReturnCoinText.text= $"{BankManager.Instance.CurrentNeedReturn.Value}";
        };
        BankManager.Instance.AdvanceInterest.OnValueChanged += (value) =>
        {
            _advanceInterestText.text = $"{(BankManager.Instance.AdvanceInterest.Value-1)*100}%";
        };

    }

    public override void OnEnable()
    {
        base.OnEnable();
        CloseButton.onClick.AddListener(CloseButtonClick);
        _saveMoneyButton.onClick.AddListener(SaveMoney);
        _advanceButton.onClick.AddListener(AdvanceMoney);
        //BattleManager.Instance.PauseGame();//暂停游戏
        //_bankCoinText.text = $"当前存款:{BankManager.Instance.Count}";
    }

    private void OnDisable()
    {
        //BattleManager.Instance.ResumeGame();//恢复游戏
        CloseButton.onClick.RemoveAllListeners();

        _saveMoneyButton.onClick.RemoveAllListeners();
        _advanceButton.onClick.RemoveAllListeners();
    }

    //关闭当前面板
    private void CloseButtonClick()
    {
        BattleUIManager.Instance.HidewBankPanel();
    }

    //借钱
    private void AdvanceMoney()
    {
        if (!BankManager.Instance.AdvanceMoney(500))
        {
            GameUIManager.Instance.ShowQuickTip("超出借款上限");
        }
        else
        {
            GameUIManager.Instance.ShowQuickTip("借款成功");
        }
    }

    //存钱
    private void SaveMoney()
    {
        if (CurrencyManager.Instance.SpendCoin(500))
        {
            BankManager.Instance.SaveMoney(WaveManager.Instance.CurrentRound, 500);
            GameUIManager.Instance.ShowQuickTip("存款成功");
        }
        else
        {
            GameUIManager.Instance.ShowQuickTip("金币不足");
        }
        
    }
}
