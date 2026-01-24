using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接下来n秒敌人死亡必定掉落金币，且掉落金币翻倍
/// </summary>
public class DropCoinBlessInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 2f, 2f, 2f };
    private Coroutine coroutine;

    public DropCoinBlessInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        Debug.Log("开始掉落金币祝福");
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }


    private IEnumerator CountDown()
    {
        EnemyManager.Instance.TmpEnemyDieCoinProb = 1;//100%
        CurrencyManager.Instance.CoinTmpMulti = multipliers[rarity];
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);
        blessBuffShow.Close();
        EnemyManager.Instance.TmpEnemyDieCoinProb = 0;
        CurrencyManager.Instance.CoinTmpMulti = 1;
        End();
        yield break;
    }

    public override void End()
    {
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }

}

