using UnityEngine;
using UnityEngine.UI;

public class GamePlayingPanel : MonoBehaviour
{
    [SerializeField] private Text _goldText;        // 显示当前金币数的 UI 文本组件
    [SerializeField] private Text _bankText;        // 显示当前银行内的金币数的 UI 文本组件
    [SerializeField] private Text _roundText;       // 显示当前回合信息的 UI 文本组件
    [SerializeField] private Text _enemyCountText;  //显示当前敌人数量的Text
    [SerializeField] private Button _openBankButton;  //打开银行面板的按钮
    [SerializeField] private Button _openEnemyInfoButton;  //打开敌人信息面板的按钮
    [SerializeField] private Button _openDoubleSpeedButton;  //打开两倍速的按钮
    [SerializeField] private Button _openPauseButton;  //打开暂停面板的按钮
    [SerializeField] private Button _openInfoButton;  //打开信息面板的按钮
    [SerializeField] private MySlider _mySlider;//下一波倒计时的slider

    private void OnEnable()
    {
        RefreshGoldDisplay(0);//刷新金币显示
        OnEnemyCountChanged(0);//刷新敌人显示
        OnWaveChanged(0);//刷新波次显示
        OnBankMoneyChanged(0);//刷新银行金币显示
        _openBankButton.onClick.AddListener(() => {
            BattleUIManager.Instance.ShowBankPanel();
        });
        _openPauseButton.onClick.AddListener(() => {
            BattleUIManager.Instance.ShowPausePanel();
        });
        _openEnemyInfoButton.onClick.AddListener(() => {
            BattleUIManager.Instance.ShowEnemyInfoInfoPanel();
        });
        _openInfoButton.onClick.AddListener(() => { 
            BattleUIManager.Instance.ShowBattleTowerInfoPanel();
        });
        _openDoubleSpeedButton.onClick.AddListener(() =>
        {
            BattleManager.Instance.GameSpeed.Value= BattleManager.Instance.GameSpeed.Value == 1 ? 2 : 1;
        });
        WaveManager.OnWaveChanged += OnWaveChanged;
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged += OnEnemyCountChanged;
        EnemyManager.OnAlmostNextWave += OnAlmostNextWave;
        CurrencyManager.OnCoinChange += RefreshGoldDisplay;//金币变化时也刷新金币显示
        BankManager.OnBankMoneyChanged += OnBankMoneyChanged;//银行金币数量变化时
    }

    private void OnDisable()
    {
        WaveManager.OnWaveChanged -= OnWaveChanged;
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged -= OnEnemyCountChanged;
        EnemyManager.OnAlmostNextWave -= OnAlmostNextWave;
        CurrencyManager.OnCoinChange -= RefreshGoldDisplay;//金币变化时也刷新金币显示
        BankManager.OnBankMoneyChanged -= OnBankMoneyChanged;//银行金币数量变化时

        _openBankButton.onClick.RemoveAllListeners();
        _openPauseButton.onClick.RemoveAllListeners();
        _openInfoButton.onClick.RemoveAllListeners();
        _openEnemyInfoButton.onClick.RemoveAllListeners();
        _openDoubleSpeedButton.onClick.RemoveAllListeners();
    }


    #region 公共方法
    /// <summary>
    /// 刷新金币显示，读取 CurrencyManager.Instance.Gold 并设置文本
    /// </summary>
    public void RefreshGoldDisplay(int amount)
    {
        _goldText.text = CurrencyManager.Instance.Gold.ToString();
    }

    /// <summary>
    /// 回合变化时，刷新UI
    /// </summary>
    /// <param name="currentRound"></param>
    private void OnWaveChanged(int currentRound)
    {
        _roundText.text =$"第 {currentRound} 波";
    }

    /// <summary>
    /// 场上敌人数量变化时，刷新UI
    /// </summary>
    /// <param name="count"></param>
    private void OnEnemyCountChanged(int count)
    {
        _enemyCountText.text = count.ToString();
    }

    //银行金币数量变化时
    private void OnBankMoneyChanged(int amount)
    {
        _bankText.text= amount.ToString();
    }

    //马上下一波
    private void OnAlmostNextWave()
    {
        _mySlider.Init(3);
    }
    #endregion
}
