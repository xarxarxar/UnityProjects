using System.Collections.Generic;
using UnityEngine;

public class BankManager : ManagerBase<BankManager>
{
    private Bindable<float> _saveInterest=new Bindable<float>();//存款利息
    private Bindable<float> _advanceInterest=new Bindable<float>();//借款利息
    private int _period =5;//过多少回合返还金币
    private Queue<BankInfo> _bankCache=new Queue<BankInfo>();//银行流水缓存
    private Bindable<int> _currentSave=new Bindable<int>();//银行拥有的金币总数
    private Bindable<int> _nextRoundGetMoney = new Bindable<int>();//下一回合会获得的金币数量
    //public static event UnityAction<int> OnBankMoneyChanged;//银行金币数量改变时

    //预支金币，也就是贷款
    private Bindable<int> _maxAdvance=new Bindable<int>();//最高可借的金额
    private Bindable<int> _currentNeedReturn=new Bindable<int>();//当前需要还多少

    /// <summary>
    /// 银行利率
    /// </summary>
    public Bindable<float> SaveInterest { get => _saveInterest; set => _saveInterest = value; }
    /// <summary>
    /// 银行汇款周期
    /// </summary>
    public int Period { get => _period; set => _period = value; }
    /// <summary>
    /// 
    /// </summary>
    public Bindable<int> CurrentSave=>_currentSave;


    /// <summary>
    /// 最高可借的金额
    /// </summary>
    public Bindable<int> MaxAdvance { get => _maxAdvance; set => _maxAdvance = value; }
    /// <summary>
    /// 当前需要还多少
    /// </summary>
    public Bindable<int> CurrentNeedReturn { get => _currentNeedReturn;}
    /// <summary>
    /// 当前借款利息
    /// </summary>
    public Bindable<float> AdvanceInterest { get => _advanceInterest; set => _advanceInterest = value; }
    /// <summary>
    /// 下一回合会获得的金币数量
    /// </summary>
    public Bindable<int> NextRoundGetMoney { get => _nextRoundGetMoney;}

    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
        Index = -1;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveChanged -= OnWaveChanged;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            SaveMoney(WaveManager.Instance.CurrentRound,100);
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public override void Init()
    {
        _currentSave.Value = 0;
        _maxAdvance.Value = 2000;
        _currentNeedReturn.Value = 0;
        _saveInterest.Value = 1.1f;
        _advanceInterest.Value = 1.5f;
        WaveManager.OnWaveChanged += OnWaveChanged;
    }

    /// <summary>
    /// 存钱
    /// </summary>
    /// <param name="round">当前回合数</param>
    /// <param name="moneyCount">要存的金币数</param>
    public void SaveMoney(int round,int moneyCount)
    {
        //CurrencyManager.Instance.SpendCoin(moneyCount);
        _currentSave.Value += moneyCount;
        _bankCache.Enqueue(new BankInfo(round, round + _period, moneyCount, _saveInterest.Value));
    }

    /// <summary>
    /// 预支金币
    /// </summary>
    /// <param name="amount">借钱的数额</param>
    /// <returns></returns>
    public bool AdvanceMoney(int amount)
    {
        if(_currentNeedReturn.Value>=_maxAdvance.Value)
        {
            return false;
        }
        if(_currentNeedReturn.Value+ amount > _maxAdvance.Value)
        {
            return false;
        }
        CurrencyManager.Instance.AddCoin(amount);
        _currentNeedReturn.Value += Mathf.RoundToInt(amount * _advanceInterest.Value);
        return true;
    }

    /// <summary>
    /// 还钱
    /// </summary>
    /// <param name="amount">还的数额</param>
    public void ReturnMoney(int amount)
    {
        if (_currentNeedReturn.Value - amount <= 0)
        {
            _currentNeedReturn.Value = 0;
        }
        else
        {
            _currentNeedReturn.Value -= amount;
        }
        
    }

    /// <summary>
    /// 回合更新事件
    /// </summary>
    private void OnWaveChanged(int round)
    {
        // 先计算下一回合的预期返还
        _nextRoundGetMoney.Value = 0;
        foreach (var deposit in _bankCache)
        {
            if (deposit._overRound == round + 1)
            {
                _nextRoundGetMoney.Value += Mathf.RoundToInt(deposit._coinCount * deposit._currentInterest);
            }
        }

        // 正常结算本回合到期存款
        // 循环检查所有到期存款
        while (_bankCache.Count > 0)
        {
            BankInfo firstDeposit = _bankCache.Peek();

            if (firstDeposit._overRound == round)
            {
                // 存款到期
                BankInfo maturedDeposit = _bankCache.Dequeue();
                int returnAmount = Mathf.RoundToInt(maturedDeposit._coinCount * maturedDeposit._currentInterest);
                _currentSave.Value -= maturedDeposit._coinCount;
                CurrencyManager.Instance.AddCoin(returnAmount);
                Debug.Log($"存款到期！返还 {returnAmount} 金币 " +
                         $"(本金: {maturedDeposit._coinCount}, 利息率: {maturedDeposit._currentInterest})");
            }
            else
            {
                // 后面的存款还未到期
                break;
            }
        }
    }
}

public class BankInfo
{
    public int _currentRound;//存钱时的回合
    public int _overRound;//存钱结束的回合
    public int _coinCount;//存入的金币数
    public float _currentInterest;//存钱时的利息

    public BankInfo(int currentRound,int overRound,int coinCount,float currentInterest)
    {
        _currentRound=currentRound;
        _overRound=overRound;
        _coinCount =coinCount;
        _currentInterest=currentInterest;
    }
}
