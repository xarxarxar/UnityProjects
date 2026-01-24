using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在n秒内，城墙受到的伤害越高，炮塔获得的攻速和伤害越高
/// </summary>
public class CrystalDamageChangeTowerInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 1.5f, 2.0f, 2.5f };
    private float totalDamage;//每受一点伤害，炮塔伤害和攻速都增加百分之0.2%
    private Coroutine coroutine;
    private BuffModifier _buffMod;

    public CrystalDamageChangeTowerInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        totalDamage = 0;
        Crystal.OnCrystalDamaged += OnCrystalDamaged;
        _buffMod = new BuffModifier { Value = 0, Source = this };
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        Debug.Log("开始水晶伤害转化祝福");
        TowerManager.Instance.AtkBuff.AddChain.AddBuff(_buffMod);
        TowerManager.Instance.AtkRateBuff.AddChain.AddBuff(_buffMod);
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);//等待
        blessBuffShow.Close();
        End();
    }

    private void OnCrystalDamaged(int damage)
    {
        totalDamage += damage;
        _buffMod.Value = totalDamage / 500;
    }


    public override void End()
    {
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        TowerManager.Instance.AtkBuff.AddChain.RemoveBuff(this);
        TowerManager.Instance.AtkRateBuff.AddChain.RemoveBuff(this);
        Crystal.OnCrystalDamaged -= OnCrystalDamaged;
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }
}
