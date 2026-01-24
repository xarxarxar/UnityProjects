using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在n秒内，每消耗10金币，炮塔伤害和攻速增加1%，最高200%
/// </summary>
public class CoinToTowerInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 1f, 1.5f, 2.0f };
    private int totalCoin;
    private Coroutine coroutine;
    private BuffModifier _buffMod;

    public CoinToTowerInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        Debug.Log("开始金币转化炮塔伤害祝福");
        totalCoin = 0;
        _buffMod=new BuffModifier { Value=0,Source=this};
        CurrencyManager.OnCoinChange += OnCoinChange;
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private void OnCoinChange(int delta)
    {
        if (delta < 0)//只有小于0，才是消耗
        {
            totalCoin += Mathf.Abs(delta);
        }

        _buffMod.Value =Mathf.Min( (totalCoin/10)*0.01f, multipliers[rarity]);

    }

    private IEnumerator CountDown()
    {
        TowerManager.Instance.AtkBuff.AddChain.AddBuff(_buffMod);
        TowerManager.Instance.AtkRateBuff.AddChain.AddBuff(_buffMod);
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);//等待n秒
        blessBuffShow.Close();
        End();
        yield break;
    }

    public override void End()
    {
        CurrencyManager.OnCoinChange -= OnCoinChange;
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        TowerManager.Instance.AtkBuff.AddChain.RemoveBuff(this);
        TowerManager.Instance.AtkRateBuff.AddChain.RemoveBuff(this);
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }
}
