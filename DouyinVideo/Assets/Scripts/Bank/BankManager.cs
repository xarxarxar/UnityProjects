using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BankManager : ManagerBase<BankManager>,IManager
{
    private float _interest=1.2f;//利息
    private int _period =3;//过多少回合返还金币
    private Queue<BankInfo> _bankCache=new Queue<BankInfo>();//银行流水缓存
    private int _count=0;//银行拥有的金币总数

    public static event UnityAction<int> OnBankMoneyChanged;//银行金币数量改变时

    /// <summary>
    /// 银行利率
    /// </summary>
    public float Interest { get => _interest; set => _interest = value; }
    /// <summary>
    /// 银行汇款周期
    /// </summary>
    public int Period { get => _period; set => _period = value; }
    /// <summary>
    /// 
    /// </summary>
    public int Count 
    { 
        get => _count;
        set
        {
            if(_count != value)
            {
                _count = value;
                OnBankMoneyChanged?.Invoke(value);
            }
        } 
    }

    protected override void Awake()
    {
        base.Awake();
        _stage=InitStage.InBattle;
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
        Count = 0;
        WaveManager.OnWaveChanged += OnWaveChanged;
    }

    /// <summary>
    /// 存钱,
    /// </summary>
    /// <param name="round">当前回合数</param>
    /// <param name="moneyCount">要存的金币数</param>
    public void SaveMoney(int round,int moneyCount)
    {
        //CurrencyManager.Instance.SpendCoin(moneyCount);
        Count += moneyCount;
        _bankCache.Enqueue(new BankInfo(round, round + _period, moneyCount, _interest));
    }


    /// <summary>
    /// 回合更新事件
    /// </summary>
    private void OnWaveChanged(int round)
    {
        //int maturedCount = 0;
        // 循环检查所有到期存款
        while (_bankCache.Count > 0)
        {
            BankInfo firstDeposit = _bankCache.Peek();

            if (firstDeposit._overRound == round)
            {
                // 存款到期
                BankInfo maturedDeposit = _bankCache.Dequeue();
                int returnAmount = Mathf.RoundToInt(maturedDeposit._coinCount * maturedDeposit._currentInterest);
                Count -= maturedDeposit._coinCount;
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
