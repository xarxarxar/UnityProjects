using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 接下来n秒内必定暴击，且暴击伤害翻倍
/// </summary>

public class CriticalBlessInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 2f, 2f, 2f };
    private Coroutine coroutine;

    public CriticalBlessInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        Debug.Log("开始必定暴击祝福");
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
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

    private IEnumerator CountDown()
    {
        TowerManager.Instance.CritProbBuff.AddChain.AddBuff(new BuffModifier { Value = 1, Source = this });
        TowerManager.Instance.CritMultBuff.MultiChain.AddBuff(new BuffModifier { Value = multipliers[rarity], Source = this });
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);
        blessBuffShow.Close();
        TowerManager.Instance.CritProbBuff.AddChain.RemoveBuff(this);
        TowerManager.Instance.CritMultBuff.MultiChain.RemoveBuff(this);
        End();
        yield break;
    }
}


