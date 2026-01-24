using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// n秒内，炮塔伤害的10%/15%/20%转化为城墙血量
/// </summary>
public class DamageToCrystalHpInstance : BlessInstance
{
    private float[] multipliers = new float[3] { 0.1f, 0.15f, 0.2f };
    private float totalDamage;
    private Coroutine coroutine;

    public DamageToCrystalHpInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        totalDamage = 0;
        Enemy.OnEnemyDamaged += OnEnemyDamaged;
        coroutine = BlessManager.Instance.StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        Debug.Log("开始伤害转为水晶血量祝福");
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}：</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        yield return TimerUtility.WaitForGameSeconds(config.times[rarity]);//等待
        blessBuffShow.Close();
        End();
    }

    private void OnEnemyDamaged(Enemy enemy,bool isCri,int damage)
    {
        totalDamage += damage;
        if (totalDamage >= 10)
        {
            Recover();//
            totalDamage = 0;
        }
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
        Enemy.OnEnemyDamaged -= OnEnemyDamaged;
        blessBuffShow.Close();
        BlessManager.Instance.RemoveBlessInstance(this);
    }
}
