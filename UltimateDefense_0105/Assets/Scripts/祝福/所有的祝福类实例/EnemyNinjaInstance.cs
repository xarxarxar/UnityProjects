using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNinjaInstance : BlessInstance
{

    public EnemyNinjaInstance(Bless config, int rarity)
        : base(config, rarity) { }

    public override void Start()
    {
        blessBuffShow = BlessManager.Instance.BlessBuffShowPool.Get();
        blessBuffShow.Init($"<color=#FFD700>{config.BlessName}£º</color>{config.Descriptions[rarity]}", config.sprite, config.times[rarity]);
        BattleUIManager.Instance.knifeSwipe.Init(config.times[rarity]);
        BattleUIManager.Instance.knifeSwipe.OnNinjaEnd -= End;
        BattleUIManager.Instance.knifeSwipe.OnNinjaEnd += End;
    }
    public override void End()
    {
        blessBuffShow.Close();
    }
}
