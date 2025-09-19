using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingPanel : MonoBehaviour
{
    [SerializeField] private Text _goldText;        // 显示当前金币数的 UI 文本组件
    [SerializeField] private Text _goldAnimText;        // 显示获得的金币数的 UI 文本组件
    [SerializeField] private Text _roundText;       // 显示当前回合信息的 UI 文本组件
    [SerializeField] private Text _enemyCountText;  //显示当前敌人数量的Text
    [SerializeField] private BindableButton _openDoubleSpeedButton;  //打开两倍速的按钮
    [SerializeField] private BindableButton _openPauseButton;  //打开暂停面板的按钮
    [SerializeField] private MySlider _mySlider;//下一波倒计时的slider

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            BattleUIManager.Instance.ShowPausePanel();
        }
    }

    private void OnEnable()
    {
        RefreshGoldDisplay(0);//刷新金币显示
        OnEnemyCountChanged(0);//刷新敌人显示
        OnWaveChanged(1);//刷新波次显示
        _goldAnimText.gameObject.SetActive(false);
        _openPauseButton.AddListener(() => {
            BattleUIManager.Instance.ShowPausePanel();
        });
        _openDoubleSpeedButton.AddListener(() =>
        {
            int speed = BattleManager.Instance.GameSpeed.Value == 1 ? 2 : 1;
            BattleManager.Instance.SetGameSpeed(speed);
        });
        WaveManager.OnWaveChanged += OnWaveChanged;
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged += OnEnemyCountChanged;
        EnemyManager.OnAlmostNextWave += OnAlmostNextWave;
        CurrencyManager.OnCoinChange += RefreshGoldDisplay;//金币变化时也刷新金币显示
        //BankManager.Instance.CurrentSave.OnValueChanged += OnBankMoneyChanged;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveChanged -= OnWaveChanged;
        EnemyManager.Instance.EnemyCurrentCount.OnValueChanged -= OnEnemyCountChanged;
        EnemyManager.OnAlmostNextWave -= OnAlmostNextWave;
        CurrencyManager.OnCoinChange -= RefreshGoldDisplay;//金币变化时也刷新金币显示

        _openPauseButton.RemoveAllListeners();
        _openDoubleSpeedButton.RemoveAllListeners();
    }


    #region 公共方法
    /// <summary>
    /// 刷新金币显示，读取 CurrencyManager.Instance.Gold 并设置文本
    /// </summary>
    public void RefreshGoldDisplay(int amount)
    {
        if (amount > 0)
        {
            _goldAnimText.text = $"+{amount}";
            PlayGoldAnim();
        }
        
        UIUtils.PlayNumberAnimation(_goldText, CurrencyManager.Instance.Gold,0.5f);
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


    //马上下一波
    private void OnAlmostNextWave()
    {
        _mySlider.StartCountDown(3);
    }

    Sequence GoldAnimSeq;
    public void PlayGoldAnim()
    {
        GoldAnimSeq.Kill();

        _goldAnimText.gameObject.SetActive(true);
        // 确保初始状态
        _goldAnimText.transform.localScale = Vector3.zero;
        _goldAnimText.color = new Color(
            _goldAnimText.color.r,
            _goldAnimText.color.g,
            _goldAnimText.color.b,
            0f
        );

        GoldAnimSeq = DOTween.Sequence();
        // 初始化时同步 GameSpeed
        GoldAnimSeq.timeScale = BattleManager.Instance.GameSpeed.Value;

        BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        // 临时订阅
        void OnSpeedChanged(int speed)
        {
            if (GoldAnimSeq != null && GoldAnimSeq.IsActive())
                GoldAnimSeq.timeScale = speed;
        }
        BattleManager.Instance.GameSpeed.OnValueChanged += OnSpeedChanged;
        // 1. scale 从 0 到 1
        GoldAnimSeq.Append(_goldAnimText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack));

        // 2. 透明度 0 → 1
        GoldAnimSeq.Join(_goldAnimText.DOFade(1f, 0.3f));

        // 3. 保持 2 秒
        GoldAnimSeq.AppendInterval(2f);

        // 4. 渐渐消失
        GoldAnimSeq.Append(_goldAnimText.DOFade(0f, 0.5f));

        // 5. 动画完成时可以回收或隐藏
        GoldAnimSeq.OnComplete(() =>
        {
            BattleManager.Instance.GameSpeed.OnValueChanged -= OnSpeedChanged;
        });
    }
    #endregion
}
