using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : ManagerBase<CurrencyManager>
{
    public List<PlayerSkillBase> playerSkillBases = new List<PlayerSkillBase>();
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;
        Index = -1;
    }

    public override void Init()
    {
        for (int i = 0; i < playerSkillBases.Count; i++) 
        { 
            playerSkillBases[i].Init(); 
        }
    }
}
