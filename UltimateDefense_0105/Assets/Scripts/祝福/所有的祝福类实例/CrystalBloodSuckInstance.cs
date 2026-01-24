using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 城墙吸血，城墙在n秒后，恢复在此期间受到伤害的n%的血量,超出部分的一半转为护盾值
/// </summary>

public class CrystalBloodSuckInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 1.5f, 2.0f, 2.5f };
    private float totalDamage;
    private Coroutine coroutine;

    public CrystalBloodSuckInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        totalDamage = 0;
        Crystal.OnCrystalDamaged += OnCrystalDamaged;
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        Debug.Log("开始水晶吸血祝福");
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);//等待
        blessBuffShow.Close();
        Recover();
        End();
    }

    private void OnCrystalDamaged(int damage)
    {
        totalDamage += damage;
    }

    private void Recover()
    {
        float totalRecover = totalDamage * multipliers[rarity];

        int needHP = Crystal.Instance.MaxHP.Value - Crystal.Instance.CurrentHP.Value;

        if (totalRecover > needHP)
        {
            Crystal.Instance.Recover(needHP);
            Crystal.Instance.AddShield(Mathf.RoundToInt((totalRecover - needHP) / 2));
        }
        else
        {
            Crystal.Instance.Recover(Mathf.RoundToInt(totalRecover));
        }
    }

    public override void End()
    {
        if (coroutine != null)
        {
            BlessManager.Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        Crystal.OnCrystalDamaged -= OnCrystalDamaged;
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }
}


