using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonManager : ManagerBase<SummonManager>
{
    public Transform SummonParent;//召唤物的父物体
    public SunFlower SunFlower;//太阳花
    public NormalCrystal NormalCrystal;//普通的墙

    protected override void Awake()
    {
        base.Awake();
        _stage = InitStage.InBattle;
        Index = 2;
    }

    public override void Init()
    {

    }
}
