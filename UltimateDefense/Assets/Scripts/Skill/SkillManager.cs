using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : ManagerBase<CurrencyManager>, IManager
{
    public List<PlayerSkillBase> playerSkillBases = new List<PlayerSkillBase>();
    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;
    }

    public override void Init()
    {
        for (int i = 0; i < playerSkillBases.Count; i++) 
        { 
            playerSkillBases[i].Init(); 
        }
    }
}
